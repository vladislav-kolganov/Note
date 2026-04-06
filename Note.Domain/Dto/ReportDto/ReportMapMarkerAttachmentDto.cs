namespace Note.Domain.Dto.ReportDto;

/// <summary>
/// Вложение маркера на карте.
/// </summary>
public record ReportMapMarkerAttachmentDto(
    long Id,
    string FileName,
    string ContentType,
    string? Description,
    string Base64Content
);