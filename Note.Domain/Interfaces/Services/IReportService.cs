using Note.Domain.Dto.ReportDto;
using Note.Domain.Result;

namespace Note.Domain.Interfaces.Services;

/// <summary>
/// Сервис отвечающий за работу с доменной части отчёта (Report).
/// </summary>
public interface IReportService

{
    ///Получение всех отчётов пользователя.
    Task<CollectionResult<ReportDto>> GetResultAsync(long userId);

    /// <summary>
    /// Получение отчёта по идентификатору.
    /// </summary>
    /// <param name="id">Id отчёта.</param>
    /// <returns></returns>
    Task<BaseResult<ReportDto>> GetReportAsync(long id);

    /// <summary>
    /// Создание отчёта с базовыми параметрами
    /// </summary>
    /// <param name="dto">Дто создания отчёта.</param>
    /// <returns>Дто отчёта.</returns>
    Task<BaseResult<ReportDto>> CreateReportAsync(CreateReportDto dto);

    /// <summary>
    /// Удаление отчёта по идентификатору.
    /// </summary>
    /// <param name="id">Id отчёта.</param>
    /// <returns>Дто отчёта.</returns>
    Task<BaseResult<ReportDto>> DeleteReportAsync(long id);

    /// <summary>
    /// Обновление отчёта по идентификатору.
    /// </summary>
    /// <param name="dto">Дто обновления отчёта.</param>
    /// <returns>Дто отчёта.</returns>
    Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto dto);
}
