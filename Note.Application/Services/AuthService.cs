using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Note.Application.Resources;
using Note.Domain.Dto;
using Note.Domain.Dto.UserDto;
using Note.Domain.Entity;
using Note.Domain.Enum;
using Note.Domain.Interfaces.Repositories;
using Note.Domain.Interfaces.Services;
using Note.Domain.Result;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Serilog;

namespace Note.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseRepository<User> _userRepositoory;
        private readonly IBaseRepository<UserToken> _userTokenRepository;
        private readonly ILogger _logger;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AuthService(IBaseRepository<User> userRepositoory,ILogger logger, IMapper mapper, IBaseRepository<UserToken> userTokenRepository, ITokenService tokenService)
        {
            _userRepositoory = userRepositoory;
            _logger = logger;
            _mapper = mapper;
            _userTokenRepository = userTokenRepository;
            _tokenService = tokenService;
        }
        public async Task<BaseResult<TokenDto>> Login(LoginUserDto dto)
        {
            try
            {
                var user = await _userRepositoory.GetAll().FirstOrDefaultAsync(x => x.Login == dto.Login);
                if (user == null)
                {
                    return new BaseResult<TokenDto>()
                    {
                        ErrorMessage = ErrorMessage.UserNotFound,
                        ErrorCode = (int)ErrorCodes.UserNotFound
                    };
                }

                var isVerifyPassword = IsVerifyPassword(user.Password, dto.Password);

                if (!isVerifyPassword)
                {
                    return new BaseResult<TokenDto>()
                    {
                        ErrorMessage = ErrorMessage.PasswordIsWrong,
                        ErrorCode = (int)ErrorCodes.PasswordIsWrong
                    };
                }
                var userToken = await _userTokenRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == user.Id);
             
                var claims = new List<Claim>()
                {
                    new Claim ( ClaimTypes.Name, dto.Login),
                    new Claim (ClaimTypes.Role,"User"),
                };
                var accessToken = _tokenService.GenerateAccessToken(claims);
                var refreshToken = _tokenService.GenerateRefreshToken();
                if (userToken == null)
                {
                    userToken = new UserToken()
                    {
                        UserId = user.Id,
                        RefreshToken = refreshToken,
                        RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                    };

                    await _userTokenRepository.CreateAsync(userToken);
                }
                else 
                {
                    userToken.RefreshToken = refreshToken;
                    userToken.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                    await _userTokenRepository.UpdateAsync(userToken);
                }

                return new BaseResult<TokenDto>()
                {
                    Data = new TokenDto()
                    {
                        RefreshToken = refreshToken,
                        AccessToken = accessToken
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new BaseResult<TokenDto>()
                {
                    ErrorMessage = ErrorMessage.InternalServerError,
                    ErrorCode = (int)ErrorCodes.InternalServerError
                };

            }
        }

        public async Task<BaseResult<UserDto>> Register(RegisterUserDto dto)
        {
            if (dto.Password != dto.PasswordConfirm)
            {
                return new BaseResult<UserDto>()
                {
                    ErrorMessage = ErrorMessage.PasswordNotEqualPasswordConfirm,
                    ErrorCode = (int)ErrorCodes.PasswordNotEqualPasswordConfirm
                };
            }
            var user = await _userRepositoory.GetAll().FirstOrDefaultAsync(x => x.Login == dto.Login);
            if (user != null)
            {
                return new BaseResult<UserDto>
                {
                    ErrorMessage = ErrorMessage.UserAlreadyExists,
                    ErrorCode = (int)ErrorCodes.UserAlreadyExists
                };
            }

            var hashUserPassword = HashPassword(dto.Password);

            var users = new User()
            {
                Login = dto.Login,
                Password = hashUserPassword
            };
            await _userRepositoory.CreateAsync(users);
            return new BaseResult<UserDto>
            {
                Data = _mapper.Map<UserDto>(users)
            };
        
        }

        private string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));

            return Convert.ToBase64String(bytes);
        }

        private bool IsVerifyPassword(string userHashPassword, string userPassword)
        {
            var hash = HashPassword(userPassword);

            return hash == userHashPassword;
        }
    }
}
