using Note.Domain.Dto;
using Note.Domain.Dto.UserDto;
using Note.Domain.Result;

namespace Note.Domain.Interfaces.Services
{
    /// <summary>
    /// Сервис предназначенный для регистрации/авторизации
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        /// <returns></returns>
        Task<BaseResult<UserDto>> Register( RegisterUserDto dto);
        /// <summary>
        /// Авторизация пользователя            /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<TokenDto>> Login( LoginUserDto dto);

    }
}
