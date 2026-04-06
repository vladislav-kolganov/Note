namespace Note.Domain.Dto.ReportDto;

/// <summary>
/// DTO для создания маркера на карте.
/// </summary>
public record CreateReportMapMarkerDto(
    double Latitude,
    double Longitude,
    string LocationName,
    int FireClass,
    string? Comment,
    List<CreateReportMapMarkerAttachmentDto>? Attachments
);