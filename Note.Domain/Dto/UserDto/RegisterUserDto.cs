namespace Note.Domain.Dto.UserDto;

public record RegisterUserDto(string Login, string Password, string PasswordConfirm, byte[]? Photo);
