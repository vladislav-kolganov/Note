using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Note.Application.Mapping;
using Note.Application.Services;
using Note.Domain.Dto.UserDto;
using Note.Domain.Entity;
using Note.Domain.Enum;
using Note.Domain.Interfaces.Database;
using Note.Domain.Interfaces.Repositories;
using Note.Domain.Interfaces.Services;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Note.Tests;

public class AuthServiceTest
{
    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private const string KnownPassword = "TestPass1";
    private static readonly string KnownPasswordHash = HashPassword(KnownPassword);

    private readonly IMapper _mapper = new MapperConfiguration(cfg =>
    {
        cfg.AddProfile(new ReportMapping());
        cfg.AddProfile(new UserMapping());
    }).CreateMapper();

    private AuthService CreateAuthService(
        Mock<IBaseRepository<User>>? userRepo = null,
        Mock<IBaseRepository<UserToken>>? userTokenRepo = null,
        Mock<IBaseRepository<Role>>? roleRepo = null,
        Mock<IBaseRepository<UserRole>>? userRoleRepo = null,
        Mock<IUnitOfWork>? unitOfWork = null,
        Mock<ITokenService>? tokenService = null,
        Mock<ILogger<AuthService>>? logger = null)
    {
        var mockTransaction = new Mock<IDbContextTransaction>();
        mockTransaction.Setup(t => t.CommitAsync(default)).Returns(Task.CompletedTask);
        mockTransaction.Setup(t => t.RollbackAsync(default)).Returns(Task.CompletedTask);

        var uow = unitOfWork ?? new Mock<IUnitOfWork>();
        uow.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
        uow.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        var usersRepo = userRepo ?? GetMockUserRepository();
        uow.Setup(u => u.Users).Returns(usersRepo.Object);

        var userRolesRepo = userRoleRepo ?? GetMockUserRoleRepository();
        uow.Setup(u => u.UserRoles).Returns(userRolesRepo.Object);
        uow.Setup(u => u.Roles).Returns(usersRepo.Object);

        var ts = tokenService ?? new Mock<ITokenService>();
        ts.Setup(t => t.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns("access-token");
        ts.Setup(t => t.GenerateRefreshToken()).Returns("refresh-token");

        return new AuthService(
            usersRepo.Object,
            logger?.Object ?? new Mock<ILogger<AuthService>>().Object,
            _mapper,
            userTokenRepo?.Object ?? GetMockUserTokenRepository().Object,
            ts.Object,
            roleRepo?.Object ?? GetMockRoleRepository().Object,
            uow.Object
        );
    }

    #region Mock Data

    private static List<User> GetAuthUsers()
    {
        return new List<User>
        {
            new()
            {
                Id = 1,
                Login = "TestUser1",
                Password = KnownPasswordHash,
                Role = new List<Role>
                {
                    new() { Id = 1, Name = "User" }
                },
                CreatedAt = DateTime.UtcNow.AddDays(-10),
            },
            new()
            {
                Id = 2,
                Login = "TestUser2",
                Password = HashPassword("TestPass2"),
                Role = new List<Role>(),
                CreatedAt = DateTime.UtcNow.AddDays(-5),
            }
        };
    }

    private static Mock<IBaseRepository<User>> GetMockUserRepository()
    {
        var users = GetAuthUsers().AsQueryable().BuildMockDbSet();
        var mock = new Mock<IBaseRepository<User>>();
        mock.Setup(r => r.GetAll()).Returns(() => users.Object);
        mock.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync((User u) =>
        {
            u.Id = 10;
            return u;
        });
        mock.Setup(r => r.Update(It.IsAny<User>())).Returns((User u) => u);
        mock.Setup(r => r.SaveChangeAsync()).ReturnsAsync(1);
        return mock;
    }

    private static Mock<IBaseRepository<Role>> GetMockRoleRepository()
    {
        var roles = new List<Role>
        {
            new() { Id = 1, Name = "User" }
        }.AsQueryable().BuildMockDbSet();

        var mock = new Mock<IBaseRepository<Role>>();
        mock.Setup(r => r.GetAll()).Returns(() => roles.Object);
        return mock;
    }

    private static Mock<IBaseRepository<UserToken>> GetMockUserTokenRepository()
    {
        var tokens = new List<UserToken>
        {
            new()
            {
                Id = 1,
                UserId = 1,
                RefreshToken = "old-refresh-token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
            }
        }.AsQueryable().BuildMockDbSet();

        var mock = new Mock<IBaseRepository<UserToken>>();
        mock.Setup(r => r.GetAll()).Returns(() => tokens.Object);
        mock.Setup(r => r.CreateAsync(It.IsAny<UserToken>())).ReturnsAsync((UserToken t) => t);
        mock.Setup(r => r.Update(It.IsAny<UserToken>())).Returns((UserToken t) => t);
        mock.Setup(r => r.SaveChangeAsync()).ReturnsAsync(1);
        return mock;
    }

    private static Mock<IBaseRepository<UserRole>> GetMockUserRoleRepository()
    {
        var userRoles = new List<UserRole>
        {
            new() { UserId = 1, RoleId = 1 }
        }.AsQueryable().BuildMockDbSet();

        var mock = new Mock<IBaseRepository<UserRole>>();
        mock.Setup(r => r.GetAll()).Returns(() => userRoles.Object);
        mock.Setup(r => r.CreateAsync(It.IsAny<UserRole>())).ReturnsAsync((UserRole ur) => ur);
        mock.Setup(r => r.SaveChangeAsync()).ReturnsAsync(1);
        return mock;
    }

    #endregion

    #region Login

    [Fact]
    public async Task Login_ShouldReturnToken_WhenValidCredentials()
    {
        // Arrange
        var service = CreateAuthService();
        var dto = new LoginUserDto("TestUser1", KnownPassword);

        // Act
        var result = await service.Login(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("access-token", result.Data.AccessToken);
        Assert.Equal("refresh-token", result.Data.RefreshToken);
        Assert.Equal(1, result.Data.UserId);
    }

    [Fact]
    public async Task Login_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        var service = CreateAuthService();
        var dto = new LoginUserDto("NonExistentUser", "password");

        // Act
        var result = await service.Login(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.UserNotFound, result.ErrorCode);
    }

    [Fact]
    public async Task Login_ShouldReturnError_WhenPasswordIsWrong()
    {
        // Arrange
        var service = CreateAuthService();
        var dto = new LoginUserDto("TestUser1", "WrongPassword");

        // Act
        var result = await service.Login(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.PasswordIsWrong, result.ErrorCode);
    }

    [Fact]
    public async Task Login_ShouldCreateToken_WhenUserHasNoToken()
    {
        // Arrange — user 2 has no token in the mock data
        var service = CreateAuthService();
        var dto = new LoginUserDto("TestUser2", "TestPass2");

        // Act
        var result = await service.Login(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("refresh-token", result.Data.RefreshToken);
    }

    #endregion

    #region Register

    [Fact]
    public async Task Register_ShouldCreateUser_WhenValidData()
    {
        // Arrange
        var service = CreateAuthService();
        var dto = new RegisterUserDto("NewUser", "Password1", "Password1", null);

        // Act
        var result = await service.Register(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("NewUser", result.Data.Login);
    }

    [Fact]
    public async Task Register_ShouldReturnError_WhenPasswordsDoNotMatch()
    {
        // Arrange
        var service = CreateAuthService();
        var dto = new RegisterUserDto("NewUser", "Password1", "DifferentPassword", null);

        // Act
        var result = await service.Register(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.PasswordNotEqualPasswordConfirm, result.ErrorCode);
    }

    [Fact]
    public async Task Register_ShouldReturnError_WhenLoginTooShort()
    {
        // Arrange
        var service = CreateAuthService();
        var dto = new RegisterUserDto("ABCD", "Password1", "Password1", null);

        // Act
        var result = await service.Register(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.MinimalLengthLoginIsFiveSymbols, result.ErrorCode);
    }

    [Fact]
    public async Task Register_ShouldReturnError_WhenLoginContainsSpaces()
    {
        // Arrange
        var service = CreateAuthService();
        var dto = new RegisterUserDto("Test User", "Password1", "Password1", null);

        // Act
        var result = await service.Register(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.LoginMustNotContainSpaces, result.ErrorCode);
    }

    [Fact]
    public async Task Register_ShouldReturnError_WhenLoginInvalidCharacters()
    {
        // Arrange — '<' is not in the allowed character set
        var service = CreateAuthService();
        var dto = new RegisterUserDto("Test<User", "Password1", "Password1", null);

        // Act
        var result = await service.Register(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.LoginInvalidCharacters, result.ErrorCode);
    }

    [Fact]
    public async Task Register_ShouldReturnError_WhenLoginNoLatinLetter()
    {
        // Arrange — login with only digits and special chars
        var service = CreateAuthService();
        var dto = new RegisterUserDto("12345", "Password1", "Password1", null);

        // Act
        var result = await service.Register(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.LoginMustContainLatinLetter, result.ErrorCode);
    }

    [Fact]
    public async Task Register_ShouldReturnError_WhenUserAlreadyExists()
    {
        // Arrange
        var service = CreateAuthService();
        var dto = new RegisterUserDto("TestUser1", "Password1", "Password1", null);

        // Act
        var result = await service.Register(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.UserAlreadyExists, result.ErrorCode);
    }

    [Fact]
    public async Task Register_ShouldReturnError_WhenRoleNotFound()
    {
        // Arrange — empty role repo
        var mockRoleRepo = new Mock<IBaseRepository<Role>>();
        var empty = new List<Role>().AsQueryable().BuildMockDbSet();
        mockRoleRepo.Setup(r => r.GetAll()).Returns(() => empty.Object);

        var service = CreateAuthService(roleRepo: mockRoleRepo);
        var dto = new RegisterUserDto("NewUser", "Password1", "Password1", null);

        // Act
        var result = await service.Register(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.RoleNotFound, result.ErrorCode);
    }

    #endregion

    #region ResetPassword

    [Fact]
    public async Task ResetPassword_ShouldSucceed_WhenValidData()
    {
        // Arrange
        var service = CreateAuthService();
        var dto = new ResetPasswordUserDto("TestUser1", "NewPassword1", "NewPassword1", KnownPassword);

        // Act
        var result = await service.ResetPasswordAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnError_WhenModelIsNull()
    {
        // Arrange
        var service = CreateAuthService();

        // Act
        var result = await service.ResetPasswordAsync(null!);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.InvalidClientRequest, result.ErrorCode);
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnError_WhenLoginIsEmpty()
    {
        // Arrange
        var service = CreateAuthService();
        var dto = new ResetPasswordUserDto("", "NewPassword1", "NewPassword1", "OldPass");

        // Act
        var result = await service.ResetPasswordAsync(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.InvalidClientRequest, result.ErrorCode);
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnError_WhenPasswordsDoNotMatch()
    {
        // Arrange
        var service = CreateAuthService();
        var dto = new ResetPasswordUserDto("TestUser1", "NewPassword1", "DifferentPassword", KnownPassword);

        // Act
        var result = await service.ResetPasswordAsync(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.PasswordNotEqualPasswordConfirm, result.ErrorCode);
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        var service = CreateAuthService();
        var dto = new ResetPasswordUserDto("NonExistentUser", "NewPassword1", "NewPassword1", "OldPass");

        // Act
        var result = await service.ResetPasswordAsync(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.UserNotFound, result.ErrorCode);
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnError_WhenOldPasswordIsWrong()
    {
        // Arrange
        var service = CreateAuthService();
        var dto = new ResetPasswordUserDto("TestUser1", "NewPassword1", "NewPassword1", "WrongOldPassword");

        // Act
        var result = await service.ResetPasswordAsync(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.OldPasswordNotEqualEntryPassword, result.ErrorCode);
    }

    #endregion
}