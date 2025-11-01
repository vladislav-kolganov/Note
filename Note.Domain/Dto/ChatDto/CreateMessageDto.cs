namespace Note.Domain.Dto.ChatDto;

/// <summary>
/// Дто создания сообщения.
/// </summary>
public record CreateMessageDto(long? ChatId, long ProducerMessageId, long ConsumerMessageId, string? TextMessage, List<byte[]>? Photo);
