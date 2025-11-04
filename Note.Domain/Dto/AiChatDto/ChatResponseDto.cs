namespace Note.Domain.Dto.AiChatDto;

/// <summary>
/// Дто ответа сервиса.
/// </summary>
/// <param name="UserId">Id пользователя.</param>
/// <param name="Answer">Ответ ИИ.</param>
public record ChatResponseDto(long UserId, string Answer);
