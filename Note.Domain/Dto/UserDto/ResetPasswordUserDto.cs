namespace Note.Domain.Dto.UserDto;

/// <summary>
/// Дто обновления пользователя.
/// </summary>
/// <param name="Login">Логин пользователя.</param>
/// <param name="Password">Пароль пользователя.</param>
/// <param name="PasswordConfirm">Подтвержденный пароль пользователя.</param>
/// <param name="OldPassword">Старый пароль.</param>
public record ResetPasswordUserDto(string Login, string Password, string PasswordConfirm, string OldPassword);