using Note.Domain.Entity.ChatEntity;

namespace Note.Domain.Dto.ChatDto;

public record ChatListItemDto(
    long ChatId,
    long PartnerId,
    string PartnerLogin,
    string? LastMessageText,
    MessagePhoto[]? Photo,
    DateTime? LastMessageCreatedAt
);