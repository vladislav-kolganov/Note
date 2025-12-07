namespace Note.Domain.Dto.ChatDto;

/// <summary>
/// Дто удаления сообщений.
/// </summary>
/// <param name="UserId">Id Пользователя.</param>
/// <param name="MessageIds">Список id сообщений.</param>
public record DeleteMessageDto (long UserId, params long[] MessageIds);