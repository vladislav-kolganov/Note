using Note.Domain.Interfaces;

namespace Note.Domain.Entity.ChatEntity;

/// <summary>
/// Модель чата
/// </summary>
public class Chat : IEntityId<long>
{
    /// <summary>
    /// Id чата
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Id пользователя 1
    /// </summary>
    public long User1 { get; set; }

    /// <summary>
    /// Id пользователя 2
    /// </summary>
    public long User2 { get; set; }

    /// <summary>
    /// Сообщения в чате
    /// </summary>
    public List<Message> Messages { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
