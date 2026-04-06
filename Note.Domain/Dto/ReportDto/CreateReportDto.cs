namespace Note.Domain.Dto.ReportDto;

/// <summary>
/// Дто для создания отчёта.
/// </summary>
/// <param name="Id">Id отчёта.</param>
/// <param name="Name">Наименование отчёта.</param>
/// <param name="Description">Описание отчёта.</param>

public record CreateReportDto(
    string Name,
    string Description,
    long UserId,
    Dictionary<string, string>? Photos,
    List<CreateReportMapMarkerDto>? MapMarkers = null);