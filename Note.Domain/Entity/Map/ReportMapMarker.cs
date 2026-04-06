using Note.Domain.Interfaces;

namespace Note.Domain.Entity.Map;

/// <summary>
/// Маркер (флаг) на карте, принадлежащий отчёту.
/// </summary>
public class ReportMapMarker : IEntityId<long>
{
    /// <summary>
    /// Id маркера.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Id отчёта.
    /// </summary>
    public long ReportId { get; set; }

    /// <summary>
    /// Широта.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Долгота.
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Название местности.
    /// </summary>
    public string LocationName { get; set; }

    /// <summary>
    /// Класс пожара.
    /// </summary>
    public int FireClass { get; set; }

    /// <summary>
    /// Комментарий.
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// Дата создания.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата обновления.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Отчёт.
    /// </summary>
    public Report Report { get; set; }

    /// <summary>
    /// Вложения маркера.
    /// </summary>
    public List<ReportMapMarkerAttachment> Attachments { get; set; } = new();
}