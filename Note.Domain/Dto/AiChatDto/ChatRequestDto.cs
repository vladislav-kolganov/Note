namespace Note.Domain.Dto.AiChatDto;

/// <summary>
/// Дто запроса к сервису.
/// </summary>
/// <param name="UserId">Id пользователя.</param>
/// <param name="UserPrompt">Промт для ИИ пользователя</param>
/// <param name="SystemPrompt">Системный промт.</param>
public record ChatRequestDto(long UserId, string UserPrompt);