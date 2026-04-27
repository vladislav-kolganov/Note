using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MockQueryable.Moq;
using Moq;
using Note.Application.Services;
using Note.Domain.Dto;
using Note.Domain.Entity;
using Note.Domain.Interfaces.Repositories;
using Note.Domain.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Note.Tests;

public class TokenServiceTest
{
    private const string JwtKey = "TestSecretKeyForJwtTokenGeneration123456";
    private const string Issuer = "TestIssuer";
    private const string Audience = "TestAudience";

    private TokenService CreateTokenService(
        Mock<IBaseRepository<User>>? userRepo = null)
    {
        var settings = Options.Create(new JwtSettings
        {
            JwtKey = JwtKey,
            Issuer = Issuer,
            Audience = Audience
        });

        return new TokenService(
            settings,
            userRepo?.Object ?? GetMockUserRepository().Object,
            new Mock<ILogger<TokenService>>().Object
        );
    }

    #region Mock Data

    private static List<User> GetUsers()
    {
        return new List<User>
        {
            new()
            {
                Id = 1,
                Login = "TestUser1",
                UserToken = new UserToken
                {
                    Id = 1, UserId = 1,
                    RefreshToken = "valid-refresh-token",
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
                }
            },
            new()
            {
                Id = 2,
                Login = "TestUser2",
                UserToken = new UserToken
                {
                    Id = 2, UserId = 2,
                    RefreshToken = "expired-refresh-token",
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1)
                }
            }
        };
    }

    private static Mock<IBaseRepository<User>> GetMockUserRepository()
    {
        var users = GetUsers().AsQueryable().BuildMockDbSet();
        var mock = new Mock<IBaseRepository<User>>();
        mock.Setup(r => r.GetAll()).Returns(() => users.Object);
        mock.Setup(r => r.Update(It.IsAny<User>())).Returns((User u) => u);
        mock.Setup(r => r.SaveChangeAsync()).ReturnsAsync(1);
        return mock;
    }

    private static string GenerateTestAccessToken(string login)
    {
        var claims = new List<Claim> { new(ClaimTypes.Name, login) };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(Issuer, Audience, claims, null,
            DateTime.UtcNow.AddMinutes(10), credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateExpiredTestAccessToken(string login)
    {
        var claims = new List<Claim> { new(ClaimTypes.Name, login) };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(Issuer, Audience, claims, null,
            DateTime.UtcNow.AddMinutes(-10), credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    #endregion

    #region GenerateAccessToken

    [Fact]
    public void GenerateAccessToken_ShouldGenerateToken_WhenValidClaims()
    {
        // Arrange
        var service = CreateTokenService();
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "TestUser1"),
            new(ClaimTypes.Role, "User")
        };

        // Act
        var token = service.GenerateAccessToken(claims);

        // Assert
        Assert.False(string.IsNullOrEmpty(token));
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        Assert.Equal(Issuer, jwt.Issuer);
        Assert.Equal(Audience, jwt.Audiences.First());
    }

    #endregion

    #region GenerateRefreshToken

    [Fact]
    public void GenerateRefreshToken_ShouldGenerateNonEmptyToken()
    {
        // Arrange
        var service = CreateTokenService();

        // Act
        var token = service.GenerateRefreshToken();

        // Assert — 32 bytes → 44 chars base64
        Assert.False(string.IsNullOrEmpty(token));
        Assert.Equal(44, token.Length);
        Assert.NotNull(Convert.FromBase64String(token));
    }

    [Fact]
    public void GenerateRefreshToken_ShouldGenerateUniqueTokens()
    {
        // Arrange
        var service = CreateTokenService();

        // Act
        var token1 = service.GenerateRefreshToken();
        var token2 = service.GenerateRefreshToken();

        // Assert
        Assert.NotEqual(token1, token2);
    }

    #endregion

    #region GetPrincipalFromExipredToken

    [Fact]
    public void GetPrincipalFromExipredToken_ShouldReturnPrincipal_WhenTokenIsValid()
    {
        // Arrange
        var service = CreateTokenService();
        var accessToken = GenerateTestAccessToken("TestUser1");

        // Act
        var principal = service.GetPrincipalFromExipredToken(accessToken);

        // Assert
        Assert.NotNull(principal);
        Assert.Equal("TestUser1", principal.Identity?.Name);
    }

    [Fact]
    public void GetPrincipalFromExipredToken_ShouldThrow_WhenTokenIsMalformed()
    {
        // Arrange
        var service = CreateTokenService();

        // Act & Assert
        Assert.Throws<SecurityTokenMalformedException>(
            () => service.GetPrincipalFromExipredToken("invalid-token"));
    }

    [Fact]
    public void GetPrincipalFromExipredToken_ShouldThrow_WhenTokenIsExpired()
    {
        // Arrange — ValidateLifetime = true means expired tokens are rejected
        // (bug: method name says "Expired" but ValidateLifetime is true)
        var service = CreateTokenService();
        var expiredToken = GenerateExpiredTestAccessToken("TestUser1");

        // Act & Assert
        Assert.ThrowsAny<SecurityTokenException>(
            () => service.GetPrincipalFromExipredToken(expiredToken));
    }

    #endregion

    #region RefreshToken

    [Fact]
    public async Task RefreshToken_ShouldReturnNewTokens_WhenValid()
    {
        // Arrange
        var service = CreateTokenService();
        var accessToken = GenerateTestAccessToken("TestUser1");
        var dto = new TokenDto
        {
            AccessToken = accessToken,
            RefreshToken = "valid-refresh-token"
        };

        // Act
        var result = await service.RefreshToken(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.False(string.IsNullOrEmpty(result.Data.AccessToken));
        Assert.False(string.IsNullOrEmpty(result.Data.RefreshToken));
        Assert.NotEqual("valid-refresh-token", result.Data.RefreshToken);
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange — access token has login "NonExistentUser" which is not in mock repo
        var service = CreateTokenService();
        var accessToken = GenerateTestAccessToken("NonExistentUser");
        var dto = new TokenDto
        {
            AccessToken = accessToken,
            RefreshToken = "some-refresh-token"
        };

        // Act
        var result = await service.RefreshToken(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnError_WhenRefreshTokenMismatch()
    {
        // Arrange — user has "valid-refresh-token" but we pass "wrong-token"
        var service = CreateTokenService();
        var accessToken = GenerateTestAccessToken("TestUser1");
        var dto = new TokenDto
        {
            AccessToken = accessToken,
            RefreshToken = "wrong-token"
        };

        // Act
        var result = await service.RefreshToken(dto);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnError_WhenRefreshTokenExpired()
    {
        // Arrange — TestUser2 has RefreshTokenExpiryTime in the past
        var service = CreateTokenService();
        var accessToken = GenerateTestAccessToken("TestUser2");
        var dto = new TokenDto
        {
            AccessToken = accessToken,
            RefreshToken = "expired-refresh-token"
        };

        // Act
        var result = await service.RefreshToken(dto);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnError_WhenAccessTokenMalformed()
    {
        // Arrange
        var service = CreateTokenService();
        var dto = new TokenDto
        {
            AccessToken = "malformed-token",
            RefreshToken = "some-refresh-token"
        };

        // Act
        var result = await service.RefreshToken(dto);

        // Assert — SecurityTokenException caught, returns error via LogErrorHelper
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnError_WhenAccessTokenExpired()
    {
        // Arrange — expired access token causes GetPrincipalFromExipredToken to throw
        // because ValidateLifetime = true (bug: should be false for refresh flow)
        var service = CreateTokenService();
        var expiredToken = GenerateExpiredTestAccessToken("TestUser1");
        var dto = new TokenDto
        {
            AccessToken = expiredToken,
            RefreshToken = "valid-refresh-token"
        };

        // Act
        var result = await service.RefreshToken(dto);

        // Assert — SecurityTokenException caught
        Assert.False(result.IsSuccess);
    }

    #endregion
}