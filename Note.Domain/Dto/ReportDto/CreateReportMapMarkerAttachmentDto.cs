namespace Note.Domain.Dto.ReportDto;

/// <summary>
/// DTO для добавления вложения к маркеру.
/// </summary>
public record CreateReportMapMarkerAttachmentDto(
    string FileName,
    string ContentType,
    string Base64Content,
    string? Description
);