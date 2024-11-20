﻿    using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Note.Application.Resources;
using Note.Domain.Dto;
using Note.Domain.Entity;
using Note.Domain.Interfaces.Repositories;
using Note.Domain.Interfaces.Services;
using Note.Domain.Result;
using Note.Domain.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Note.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly string _jwtKey;
        private readonly string _issuer;
        private readonly string _audience;

        public TokenService(IOptions<JwtSettings> options, IBaseRepository<User> userRepository)
        {
            _jwtKey = options.Value.JwtKey;
            _issuer = options.Value.Issuer;
            _audience = options.Value.Audience;
            _userRepository = userRepository;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            var securityToken =
                new JwtSecurityToken(_issuer, _audience, claims, null, DateTime.UtcNow.AddMinutes(10), credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return token;
        }

        public string GenerateRefreshToken()
        {
            var randomNumbers = new byte[32];
            var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumbers);
            return Convert.ToBase64String(randomNumbers);
        }

        public ClaimsPrincipal GetPrincipalFromExipredToken(string accessToken)
        {
            var tokenValidationParametrs = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)),
                ValidateLifetime = true,
                ValidAudience = _audience,
                ValidIssuer = _issuer
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var claimsPrincipal = tokenHandler.ValidateToken(accessToken, tokenValidationParametrs, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException(ErrorMessage.InvalidToken);
            }
            return claimsPrincipal;
        }

        public async Task<BaseResult<TokenDto>> RefreshToken(TokenDto dto)
        {
            var accessToken = dto.AccessToken;
            var refreshToken = dto.RefreshToken;

            var claimsPrincipal = GetPrincipalFromExipredToken(accessToken);
            var userName = claimsPrincipal.Identity?.Name;
            var user = await _userRepository.GetAll().Include(x => x.UserToken).FirstOrDefaultAsync(x => x.Login == userName);

            if (user == null || user.UserToken.RefreshToken != refreshToken || user.UserToken.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return new BaseResult<TokenDto>()
                {
                    ErrorMessage = ErrorMessage.InvalidClientRequest
                };
            }
            var newAccessToken = GenerateAccessToken(claimsPrincipal.Claims);
            var newRefreshToken = GenerateRefreshToken();

            user.UserToken.RefreshToken = newRefreshToken;

           _userRepository.Update(user);
            await _userRepository.SaveChangeAsync();

            return new BaseResult<TokenDto>()
            {
                Data = new TokenDto() 
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                }
            };
        }
    }
}
