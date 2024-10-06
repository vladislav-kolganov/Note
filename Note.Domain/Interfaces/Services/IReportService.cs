using Note.Domain.Dto.ReportDto;
using Note.Domain.Result;

namespace Note.Domain.Interfaces.Services
{
    // Сервис отвечающий за работу с доменной части отчёта (Report)
    public interface IReportService
    {   // получение всех отчётов пользователя
        Task<CollectionResult<ReportDto>> GetResultAsync(long userId);
        // получение отчёта по идентификатору
        Task<BaseResult<ReportDto>> GetReportAsync(long id);
        // создание отчёта с базовыми параметрами
        Task<BaseResult<ReportDto>> CreateReportAsync(CreateReportDto dto);
        // удаление отчёта по идентификатору
        Task<BaseResult<ReportDto>> DeleteReportAsync(long id);
        // обновление отчёта по идентификатору
        Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto dto);
    }
}
