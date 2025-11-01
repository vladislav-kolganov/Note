namespace Note.Domain.Dto.ChatDto;

/// <summary>
/// Дто редактирования сообщения.
/// </summary>
/// <param name="messageId">Id сообщения.</param>
/// <param name="textMessage">Текст сообщения.</param>
public record EditMessageDto(long messageId, string textMessage);
