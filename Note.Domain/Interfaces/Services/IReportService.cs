using Note.Domain.Dto.ReportDto;
using Note.Domain.Result;

namespace Note.Domain.Interfaces.Services;

/// <summary>
/// Сервис отвечающий за работу с доменной части отчёта (Report).
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Получение отчётов пользователя, которыми он валедеет.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список дто отчётов.</returns>
    Task<CollectionResult<ReportDto>> GetUserReportsAsync(long userId);

    /// <summary>
    /// Получение расшеренных отчётоы по идентификатору пользователя.
    /// </summary>
    /// <param name="userId">Id отчёта.</param>
    Task<CollectionResult<ReportDto>> GetSharedReportAsync(long userId);

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
    Task<BaseResult<bool>> DeleteReportAsync(long reportId, long userId);

    /// <summary>
    /// Обновление отчёта по идентификатору.
    /// </summary>
    /// <param name="dto">Дто обновления отчёта.</param>
    /// <returns>Дто отчёта.</returns>
    Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto dto);

    /// <summary>
    /// Шеринг отчёта.
    /// </summary>
    /// <param name="dto">Дто шеринга отчёта.</param>
    /// <returns>True, если получилось расшерить отчёт, иначе ошибки.</returns>
    Task<BaseResult<bool>> ShareReport(ShareReportDto dto);

    /// <summary>
    /// Получить отчёт по Id отчёта.
    /// </summary>
    /// <param name="reportId">Id отчёта.</param>
    /// <returns>Дто отчёта.</returns>
    Task<BaseResult<ReportDto>> GetReportById(long reportId);
}