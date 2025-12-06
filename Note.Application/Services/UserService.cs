using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Note.Application.Resources;
using Note.Application.Services.Extensions;
using Note.Application.Services.Helpers;
using Note.Domain.Dto.ChatDto;
using Note.Domain.Dto.UserDto;
using Note.Domain.Entity;
using Note.Domain.Enum;
using Note.Domain.Interfaces.Database;
using Note.Domain.Interfaces.Repositories;
using Note.Domain.Interfaces.Services;
using Note.Domain.Result;

namespace Note.Application.Services;

/// <summary>
/// Сервис работы с пользователем.
/// </summary>
public class UserService : IUserService
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<UserRole> _userRoleRepository;
    private readonly IAuthService _authService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IBaseRepository<User> userRepository,
        IBaseRepository<UserRole> userRoleService,
        IAuthService authService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleService;
        _authService = authService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Метод создания пользователя.
    /// </summary>
    /// <param name="model">Модель пользователя.</param>
    public async Task<BaseResult<UserDto>> Create(RegisterUserDto model)
    {
        try
        {
            if (model == null)
            {
                return new BaseResult<UserDto>
                {
                    ErrorMessage = ErrorMessage.InvalidClientRequest,
                    ErrorCode = (int)ErrorCodes.InvalidClientRequest
                };
            }

            var user = await _userRepository.GetAll().
                FirstOrDefaultAsync(x => x.Login == model.Login);

            if (user != null)
            {
                return new BaseResult<UserDto>
                {
                    ErrorMessage = ErrorMessage.UserAlreadyExists,
                    ErrorCode = (int)ErrorCodes.UserAlreadyExists
                };
            }

            return await _authService.Register(new RegisterUserDto
                (
                    Login: model.Login,
                    Password: model.Password,
                    PasswordConfirm: model.PasswordConfirm,
                    Photo: model.Photo)
                );
        }
        catch (Exception ex)
        {
            return LogErrorHelper<UserDto>.LogException(ex.Message, _logger);
        }
    }

    /// <summary>
    /// Метод удаления пользователя
    /// </summary>
    /// <param name="id">Id пользователя</param>
    public async Task<BaseResult<UserDto>> Delete(long id)
    {
        var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
        {
            return new BaseResult<UserDto>
            {
                ErrorMessage = ErrorMessage.UserNotFound,
                ErrorCode = (int)ErrorCodes.UserNotFound
            };
        }
        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                _userRepository.Remove(user);
                await _unitOfWork.SaveChangeAsync();

                var userRole = await _userRoleRepository.GetAll()
                    .Where(x => x.UserId == id)
                    .ToListAsync();

                if (userRole.Any())
                {
                    userRole.ForEach(async x =>
                    {
                        _userRoleRepository.Remove(x);
                        await _unitOfWork.SaveChangeAsync();
                    });

                    await transaction.CommitAsync();

                    return new BaseResult<UserDto>
                    {
                        Data = _mapper.Map<UserDto>(user)
                    };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return LogErrorHelper<UserDto>.LogException(ex.Message, _logger);
            }
        }

        return new BaseResult<UserDto>
        {
            ErrorMessage = ErrorMessage.InternalServerError,
            ErrorCode = (int)ErrorCodes.InternalServerError
        };
    }

    public async Task<CollectionResult<UserFindDto>> FindUsersAsync(string login)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return new CollectionResult<UserFindDto>()
                {
                    ErrorMessage = ErrorMessage.UserLoginIsEmpty,
                    ErrorCode = (int)ErrorChatCodes.UserLoginIsEmpty
                };
            }

            var user = await _userRepository.GetAll().Where(user => user.Login.Contains(login)).
                                             ToArrayAsync();
            if (user.IsNullOrEmpty())
            {
                return new CollectionResult<UserFindDto>()
                {
                    ErrorMessage = ErrorMessage.UserNotFound,
                    ErrorCode = (int)ErrorCodes.UserNotFound
                };
            }
            var dto = user.Select(x => new UserFindDto(x.Login, x.Id)).ToList();

            return new CollectionResult<UserFindDto>()
            {
                Data = dto,
                Count = dto.Count,
            };
        }
        catch (Exception ex)
        {
            return LogErrorHelper<UserFindDto>.LogExceptionForCollection(ex.Message, _logger);
        }
    }

    /// <summary>
    /// Метод обновления пользователя
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<BaseResult<UserDto>> Update(UserDto model)
    {
        try
        {
            if (model == null)
            {
                return new BaseResult<UserDto>
                {
                    ErrorMessage = ErrorMessage.InvalidClientRequest,
                    ErrorCode = (int)ErrorCodes.InvalidClientRequest
                };
            }

            var user = await _userRepository.GetAll().
                FirstOrDefaultAsync(x => x.Login == model.Login);

            if (user == null)
            {
                return new BaseResult<UserDto>
                {
                    ErrorMessage = ErrorMessage.UserNotFound,
                    ErrorCode = (int)ErrorCodes.UserNotFound
                };
            }

            _userRepository.Update(user);
            await _unitOfWork.SaveChangeAsync();

            return new BaseResult<UserDto>
            {
                Data = _mapper.Map<UserDto>(user)
            };
        }
        catch (Exception ex)
        {
            return LogErrorHelper<UserDto>.LogException(ex.Message, _logger);
        }
    }
}
