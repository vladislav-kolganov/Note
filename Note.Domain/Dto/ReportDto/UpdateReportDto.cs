namespace Note.Domain.Dto.ReportDto;

/// <summary>
/// Дто для обновления отчёта.
/// </summary>
/// <param name="Id">Id отчёта.</param>
/// <param name="Name">Наименование отчёта.</param>
/// <param name="Description">Описание отчёта.</param>
/// <param name="PhotosBase64">Список фото.</param>
public record UpdateReportDto(
    long reportId,
    long userId,
    string Name,
    string Description,
    Dictionary<string, string>? Photos,
    List<CreateReportMapMarkerDto>? MapMarkers = null);