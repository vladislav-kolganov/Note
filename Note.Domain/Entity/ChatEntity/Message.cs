using Note.Domain.Interfaces;
using System.Text.Json.Serialization;

namespace Note.Domain.Entity.ChatEntity;

/// <summary>
/// Модель сообщения
/// </summary>
public class Message : IEntityId<long>
{
    /// <summary>
    /// Id сообщения
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Текст сообщения
    /// </summary>
    public string? TextMessage { get; set; }

    /// <summary>
    /// Поддержка фотографий в сообщении
    /// </summary>
    public List<MessagePhoto> Photos { get; set; } = new();

    /// <summary>
    /// Id пользователя, который отправил сообщение
    /// </summary>
    public long ProducerMessageId { get; set; }

    /// <summary>
    /// Id пользователя который получил сообщение
    /// </summary>
    public long ConsumerMessageId { get; set; }

    /// <summary>
    /// Id чата
    /// </summary>
    public long ChatId { get; set; }

    /// <summary>
    /// Модель чата
    /// </summary>
    [JsonIgnore]
    public Chat Chat { get; set; }

    /// <summary>
    /// Дата создания сообщения
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата редактирования сообщения
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
