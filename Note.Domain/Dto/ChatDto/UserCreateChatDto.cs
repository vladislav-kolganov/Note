namespace Note.Domain.Dto.ChatDto;

/// <summary>
/// Дто создания чата.
/// </summary>
/// <param name="IdUser1">Id первого пользователя.</param>
/// <param name="IdUser2">Id второго пользоваетля.</param>
/// <param name="IdChat">Id чата пользоваетелей.</param>
public record UserCreateChatDto(long IdUser1, long IdUser2, long? IdChat = null);