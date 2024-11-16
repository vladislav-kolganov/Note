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
using Serilog;
using System.ComponentModel.Design;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Note.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<User> _userRepositoory;
        private readonly IBaseRepository<UserToken> _userTokenRepository;
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly IBaseRepository<UserRole> _userRoleRepository;
        private readonly ILogger _logger;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AuthService(IBaseRepository<User> userRepositoory, ILogger logger, IMapper mapper, IBaseRepository<UserToken> userTokenRepository, ITokenService tokenService, IBaseRepository<Role> roleRepository, IBaseRepository<UserRole> userRoleRepository, IUnitOfWork unitOfWork)
        {
            _userRepositoory = userRepositoory;
            _logger = logger;
            _mapper = mapper;
            _userTokenRepository = userTokenRepository;
            _tokenService = tokenService;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseResult<TokenDto>> Login(LoginUserDto dto)
        {
            try
            {
                var user = await _userRepositoory.GetAll()
                    .Include(x => x.Role)
                    .FirstOrDefaultAsync(x => x.Login == dto.Login);

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
                var userRols = user.Role;

                var claims = userRols.Select(x => new Claim(ClaimTypes.Role, x.Name)).ToList();
                claims.Add(new Claim(ClaimTypes.Name, dto.Login));

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

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var users = new User()
                    {
                        Login = dto.Login,
                        Password = hashUserPassword
                    };
                    await _unitOfWork.Users.CreateAsync(users);
                    var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == "User");
                    if (role != null)
                    {
                        return new BaseResult<UserDto>
                        {
                            ErrorMessage = ErrorMessage.RoleNotFound,
                            ErrorCode = (int)ErrorCodes.RoleNotFound
                        };
                    }
                    UserRole userRole = new UserRole()
                    {
                        RoleId = role.Id,
                        UserId = user.Id
                    };
                    await _unitOfWork.UserRoles.CreateAsync(userRole);

                    await transaction.CommitAsync();
                    return new BaseResult<UserDto>
                    {
                        Data = _mapper.Map<UserDto>(users)
                    };
                
                }
                catch 
                {
                    await transaction.RollbackAsync();
                    return new BaseResult<UserDto>
                    {
                        ErrorMessage = ErrorMessage.InternalServerError,
                        ErrorCode = (int)ErrorCodes.InternalServerError
                    };
                }           
            }


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
