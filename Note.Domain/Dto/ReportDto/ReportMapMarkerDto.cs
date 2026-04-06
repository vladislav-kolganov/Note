namespace Note.Domain.Dto.ReportDto;

/// <summary>
/// Маркер на карте.
/// </summary>
public record ReportMapMarkerDto(
    long Id,
    double Latitude,
    double Longitude,
    string LocationName,
    int FireClass,
    string? Comment,
    ReportMapMarkerAttachmentDto[]? Attachments
);