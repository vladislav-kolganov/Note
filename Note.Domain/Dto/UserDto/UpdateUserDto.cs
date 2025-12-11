namespace Note.Domain.Dto.UserDto;

/// <summary>
/// Дто обновления пользователя.
/// </summary>
/// <param name="UserId">Id пользователя.</param>
/// <param name="Login">Логин пользователя.</param>
/// <param name="Password">Пароль пользователя.</param>
/// <param name="PasswordConfirm">Подтвержденный пароль пользователя.</param>
/// <param name="Photo">Фото пользователя.</param>
public record UpdateUserDto(long? UserId, string Login, string? Password, string? PasswordConfirm, string? Photo);