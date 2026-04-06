using Note.Domain.Entity;

namespace Note.Domain.Dto.ReportDto;

/// <summary>
/// Дто отчёта.
/// </summary>
/// <param name="Id">Id отчёта.</param>
/// <param name="Name">Наименование отчёта.</param>
/// <param name="Description">Описание отчёта.</param>
/// <param name="Photo">Фото отчёта.</param>
/// <param name="DateCreated">Дата создания отчёта.</param>
public record ReportDto(
    long Id,
    string Name,
    string Description,
    ReportPhoto[]? Photos,
    string DateCreated,
    ReportMapMarkerDto[]? MapMarkers = null);