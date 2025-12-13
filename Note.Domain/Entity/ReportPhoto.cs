using System.Text.Json.Serialization;

namespace Note.Domain.Entity;

/// <summary>
/// Модель фото для отчётов.
/// </summary>
public class ReportPhoto
{
    /// <summary>
    /// Id фото.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Id сообщения.
    /// </summary>
    public long ReportId { get; set; }

    /// <summary>
    /// Модель сообщения.
    /// </summary>
    [JsonIgnore]
    public Report Report { get; set; }

    /// <summary>
    /// Набор байтов в котором хранится фото.
    /// </summary>
    public byte[] Content { get; set; }
}