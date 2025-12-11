using Note.Domain.Dto.ChatDto;
using Note.Domain.Dto.UserDto;
using Note.Domain.Result;

namespace Note.Domain.Interfaces.Services;

/// <summary>
/// Интерфейс сервиса пользователя
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Метод создания пользователя
    /// </summary>
    /// <param name="model">Модель пользователя</param>
    Task<BaseResult<UserDto>> CreateAsync(RegisterUserDto model);

    /// <summary>
    /// Метод удаления пользователя
    /// </summary>
    /// <param name="id">Id пользователя</param>
    Task<BaseResult<UserDto>> DeleteAsync(long id);

    /// <summary>
    /// Метод обновления или удаления фото пользователя.
    /// </summary>
    /// <param name="login">Логин пользоваетеля.</param>
    Task<BaseResult<UpdateOrDeletePhotoDto>> UpdateOrDeletePhotoAsync(UpdateOrDeletePhotoDto model);

    /// <summary>
    /// Поиск пользователя по логину.
    /// </summary>
    /// <param name="login">Логин пользоваетеля.</param>
    /// <returns>Пользователи с похожим логином</returns>
    Task<CollectionResult<UserFindDto>> FindUsersAsync(string login);

    /// <summary>
    /// Поиск пользоваетеля по id.
    /// </summary>
    /// <param name="id">Id пользователя.</param>
    Task<BaseResult<GetInfoUserDto>> GetUserByIdAsync(long id);
}