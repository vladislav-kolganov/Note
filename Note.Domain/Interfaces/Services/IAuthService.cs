using Note.Domain.Dto;
using Note.Domain.Dto.UserDto;
using Note.Domain.Result;

namespace Note.Domain.Interfaces.Services
{
    /// <summary>
    /// Интерфейс сервиса предназначенный для 
    /// регистрации/авторизации пользователей.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Регистрация пользователя.
        /// </summary>
        /// <param name="dto">Дто регистрации пользователя.</param>
        Task<BaseResult<UserDto>> Register(RegisterUserDto dto);

        /// <summary>
        /// Авторизация пользователя.
        /// </summary>
        /// <param name="dto">Дто авторизации пользователя.</param>
        Task<BaseResult<TokenDto>> Login(LoginUserDto dto);

    }
}
