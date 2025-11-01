namespace Note.Domain.Dto.ChatDto;

/// <summary>
/// Дто создания сообщения.
/// </summary>
public record CreateMessageDto(long? chatId, long producerMessageId, long consumerMessageId, string? textMessage, List<byte[]>? photo);
