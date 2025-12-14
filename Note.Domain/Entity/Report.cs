using Note.Domain.Interfaces;

namespace Note.Domain.Entity;

/// <summary>
/// Модель отчёта в БД.
/// </summary>
public class Report : IEntityId<long>
{
    /// <summary>
    /// Id отчёта.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Имя отчёта.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Описание отчёта.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Поддержка фотографий в отчёте.
    /// </summary>
    public List<ReportPhoto> Photos { get; set; } = new();

    /// <summary>
    /// Время создания отчёта.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Время редактирования отчёта.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}