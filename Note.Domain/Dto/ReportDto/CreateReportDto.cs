namespace Note.Domain.Dto.ReportDto;

/// <summary>
/// Дто для создания отчёта.
/// </summary>
/// <param name="Id">Id отчёта.</param>
/// <param name="Name">Наименование отчёта.</param>
/// <param name="Description">Описание отчёта.</param>
/// <param name="PhotosBase64">Список фото.</param>
public record CreateReportDto(string Name, string Description, long UserId, Dictionary<string, string>? Photos);