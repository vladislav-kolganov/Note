namespace Note.Domain.Entity.ChatEntity;

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
    public Message Message { get; set; }

    /// <summary>
    /// Набор байтов в котором хранится фото.
    /// </summary>
    public byte[] Content { get; set; }
}
