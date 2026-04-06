using System.Text.Json.Serialization;

namespace Note.Domain.Entity.Map;

/// <summary>
/// Вложение к маркеру на карте.
/// </summary>
public class ReportMapMarkerAttachment
{
    /// <summary>
    /// Id вложения.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Id маркера.
    /// </summary>
    public long MarkerId { get; set; }

    /// <summary>
    /// Имя файла.
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Mime type файла.
    /// </summary>
    public string ContentType { get; set; }

    /// <summary>
    /// Описание файла.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Контент файла.
    /// </summary>
    public byte[] Content { get; set; }

    /// <summary>
    /// Маркер.
    /// </summary>
    [JsonIgnore]
    public ReportMapMarker Marker { get; set; }
}