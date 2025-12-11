namespace Note.Domain.Dto.UserDto;

/// <summary>
/// Дто регистрации пользователя.
/// </summary>
/// <param name="Login">Логин.</param>
/// <param name="Password">Пароль.</param>
/// <param name="PasswordConfirm">Подтверждение пароля.</param>
/// <param name="Photo">Фото.</param>
public record RegisterUserDto(string Login, string Password, string PasswordConfirm, string? Photo);
