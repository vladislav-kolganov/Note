using System.Text.Json.Serialization;

namespace Note.Domain.Entity.ChatEntity;

/// <summary>
/// Модель фото.
/// </summary>
public class MessagePhoto
{
    /// <summary>
    /// Id фото.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Id сообщения.
    /// </summary>
    public long MessageId { get; set; }

    /// <summary>
    /// Модель сообщения.
    /// </summary>
    [JsonIgnore]
    public Message Message { get; set; }

    /// <summary>
    /// Набор байтов в котором хранится фото.
    /// </summary>
    public byte[] Content { get; set; }
}