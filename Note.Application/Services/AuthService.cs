using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Note.Application.Resources;
using Note.Application.Services.Helpers;
using Note.Domain.Dto;
using Note.Domain.Dto.UserDto;
using Note.Domain.Entity;
using Note.Domain.Enum;
using Note.Domain.Interfaces.Database;
using Note.Domain.Interfaces.Repositories;
using Note.Domain.Interfaces.Services;
using Note.Domain.Result;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Note.Application.Services;

/// <summary>
/// Сервис предназначенный для регистрации/авторизации пользователей.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<UserToken> _userTokenRepository;
    private readonly IBaseRepository<Role> _roleRepository;
    private readonly IBaseRepository<UserRole> _userRoleRepository;
    private readonly ILogger<AuthService> _logger;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(
        IBaseRepository<User> userRepositoory,
        ILogger<AuthService> logger,
        IMapper mapper,
        IBaseRepository<UserToken> userTokenRepository,
        ITokenService tokenService,
        IBaseRepository<Role> roleRepository,
        IBaseRepository<UserRole> userRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepositoory;
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
            var user = await _userRepository.GetAll()
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

            var userToken = await _userTokenRepository.GetAll().
                                  FirstOrDefaultAsync(x => x.UserId == user.Id);

            var userRols = user.Role;

            var claims = userRols.Select(x => new Claim(ClaimTypes.Role, x.Name)).
                         ToList();
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

                _userTokenRepository.Update(userToken);
                await _userTokenRepository.SaveChangeAsync();
            }

            user.LastLoginDate = DateTime.UtcNow;

            _userRepository.Update(user);
            await _unitOfWork.SaveChangeAsync();

            return new BaseResult<TokenDto>()
            {
                Data = new TokenDto()
                {
                    RefreshToken = refreshToken,
                    AccessToken = accessToken,
                    UserId = user.Id
                }
            };
        }
        catch (Exception ex)
        {
            return LogErrorHelper<TokenDto>.LogException(ex.Message, _logger);
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

        if (string.IsNullOrWhiteSpace(dto.Login))
        {
            return new BaseResult<UserDto>()
            {
                ErrorMessage = ErrorMessage.UserLoginIsEmpty,
                ErrorCode = (int)ErrorCodes.InvalidClientRequest
            };
        }

        if (dto.Login.Length < 5)
        {
            return new BaseResult<UserDto>()
            {
                ErrorMessage = ErrorMessage.MinimalLengthLoginIsThreeSymbols,
                ErrorCode = (int)ErrorCodes.MinimalLengthLoginIsThreeSymbols
            };
        }

        if (dto.Login.Any(char.IsWhiteSpace))
        {
            return new BaseResult<UserDto>()
            {
                ErrorMessage = ErrorMessage.LoginMustNotContainSpaces,
                ErrorCode = (int)ErrorCodes.LoginMustNotContainSpaces
            };
        }

        if (!IsValidLoginChars(dto.Login))
        {
            return new BaseResult<UserDto>()
            {
                ErrorMessage = ErrorMessage.LoginInvalidCharacters,
                ErrorCode = (int)ErrorCodes.LoginInvalidCharacters
            };
        }

        if (!dto.Login.Any(c => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z'))
        {
            return new BaseResult<UserDto>()
            {
                ErrorMessage = ErrorMessage.LoginMustContainLatinLetter,
                ErrorCode = (int)ErrorCodes.LoginMustContainLatinLetter
            };
        }

        var user = await _userRepository.GetAll().
                         FirstOrDefaultAsync(x => x.Login == dto.Login);

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
                user = new User()
                {
                    Login = dto.Login,
                    Password = hashUserPassword,
                    Photo = dto.Photo != null ? Convert.FromBase64String(dto.Photo) : Array.Empty<byte>(),
                };

                await _unitOfWork.Users.CreateAsync(user);
                await _unitOfWork.SaveChangeAsync();

                var role = await _roleRepository.GetAll().
                                 FirstOrDefaultAsync(x => x.Name == nameof(RoleCodes.User));

                if (role == null)
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
                await _unitOfWork.SaveChangeAsync();

                await transaction.CommitAsync();

                return new BaseResult<UserDto>
                {
                    Data = _mapper.Map<UserDto>(user)
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return LogErrorHelper<UserDto>.LogException(ex.Message, _logger);
            }
        }


    }

    /// <summary>
    /// Метод обновления пользователя
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<BaseResult<ResetPasswordUserDto>> ResetPasswordAsync(ResetPasswordUserDto model)
    {
        try
        {
            if (model is null || string.IsNullOrEmpty(model.Login))
            {
                return new BaseResult<ResetPasswordUserDto>
                {
                    ErrorMessage = ErrorMessage.InvalidClientRequest,
                    ErrorCode = (int)ErrorCodes.InvalidClientRequest
                };
            }
            if (model.PasswordConfirm != model.Password)
            {
                return new BaseResult<ResetPasswordUserDto>
                {
                    ErrorMessage = ErrorMessage.PasswordNotEqualPasswordConfirm,
                    ErrorCode = (int)ErrorCodes.PasswordNotEqualPasswordConfirm
                };
            }

            var user = await _userRepository.GetAll().
                             FirstOrDefaultAsync(x => x.Login == model.Login);

            if (user is null)
            {
                return new BaseResult<ResetPasswordUserDto>
                {
                    ErrorMessage = ErrorMessage.UserNotFound,
                    ErrorCode = (int)ErrorCodes.UserNotFound
                };
            }

            var isVerifyPassword = IsVerifyPassword(user.Password, model.OldPassword);

            if (!isVerifyPassword)
            {
                return new BaseResult<ResetPasswordUserDto>
                {
                    ErrorMessage = ErrorMessage.OldPasswordNotEqualEntryPassword,
                    ErrorCode = (int)ErrorCodes.OldPasswordNotEqualEntryPassword
                };
            }

            user.Password = HashPassword(model.Password);

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangeAsync();

            return new BaseResult<ResetPasswordUserDto>
            {
                Data = model
            };
        }
        catch (Exception ex)
        {
            return LogErrorHelper<ResetPasswordUserDto>.LogException(ex.Message, _logger);
        }
    }

    /// <summary>
    /// Метод хеширования пароля.
    /// </summary>
    /// <param name="password">Строка пароля.</param>
    private string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));

        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Проверка хэш паролей пользователя.
    /// </summary>
    /// <param name="userHashPassword">Хэш пароля пользователя из БД.</param>
    /// <param name="userPassword">Введенный пароль поьлзователя.</param>
    private bool IsVerifyPassword(string userHashPassword, string userPassword)
    {
        var hash = HashPassword(userPassword);

        return hash == userHashPassword;
    }

    private static readonly HashSet<char> AllowedLoginChars = new(
    @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=+[]{};:,.?/\|");

    private static bool IsValidLoginChars(string login)
    {
        return login.All(c => AllowedLoginChars.Contains(c));
    }
}