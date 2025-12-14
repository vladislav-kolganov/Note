namespace Note.Domain.Entity;

/// <summary>
///Сущность для реализация связи многие ко многим и механики "поделиться отчётом."
/// </summary>
public class UserReport
{
    /// <summary>
    /// Id пользователя, который имеет доступ к отчёту.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Id отчёта.
    /// </summary>
    public long ReportId { get; set; }

    /// <summary>
    /// Id пользователя, который создал отчёт (владелец).
    /// </summary>
    public long OwnerId { get; set; }

    /// <summary>
    /// Удалил ли у себя пользователь отчёт.
    /// </summary>
    public bool IsDeleteForThisUser { get; set; }
}