using Note.Domain.Entity.ChatEntity;

namespace Note.Domain.Dto.ChatDto;

/// <summary>
/// Дто сообщения с дополнительной информацией.
/// </summary>
/// <param name="ChatId">Id чата.</param>
/// <param name="PartnerId">Id получателя сообщения.</param>
/// <param name="PartnerLogin">Логин получателя сообщения.</param>
/// <param name="LastMessageText">Текст последнего сообщения.</param>
/// <param name="Photo">Фото сообщения.</param>
/// <param name="LastMessageCreatedAt">Дата создания последнего сообщения.</param>
public record ChatListItemDto(
    long ChatId,
    long PartnerId,
    string PartnerLogin,
    string? LastMessageText,
    MessagePhoto[]? Photo,
    DateTime? LastMessageCreatedAt
);