namespace Note.Domain.Dto.ChatDto;

/// <summary>
/// Дто редактирования сообщения.
/// </summary>
/// <param name="MessageId">Id сообщения.</param>
/// <param name="TextMessage">Текст сообщения.</param>
public record EditMessageDto(long MessageId, string TextMessage);
