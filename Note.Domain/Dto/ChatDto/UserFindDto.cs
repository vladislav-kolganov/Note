namespace Note.Domain.Dto.ChatDto;

/// <summary>
/// Дто пользователя, которго нашли по логину.
/// </summary>
/// <param name="Login">Логин пользователя.</param>
/// <param name="Id">Id пользователя.</param>
public record UserFindDto(string Login, long Id, byte[]? Photo);