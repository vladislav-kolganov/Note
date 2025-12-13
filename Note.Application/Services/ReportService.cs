using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Note.Application.Resources;
using Note.Application.Services.Extensions;
using Note.Application.Services.Helpers;
using Note.Domain.Dto.ReportDto;
using Note.Domain.Entity;
using Note.Domain.Enum;
using Note.Domain.Interfaces.Repositories;
using Note.Domain.Interfaces.Services;
using Note.Domain.Interfaces.Validations;
using Note.Domain.Result;

namespace Note.Application.Services;

public class ReportService : IReportService
{
    private readonly IBaseRepository<Report> _reportRepository;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<ReportPhoto> _photoReportRepository;
    private readonly IReportValidator _reportValidator;
    private readonly IMapper _mappper;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        IBaseRepository<Report> reportRepository,
        IBaseRepository<User> userRepository,
        IBaseRepository<ReportPhoto> photoReportRepository,
        IReportValidator reportValidator,
        ILogger<ReportService> logger,
        IMapper mapper)
    {
        _reportRepository = reportRepository;
        _userRepository = userRepository;
        _reportValidator = reportValidator;
        _logger = logger;
        _mappper = mapper;
        _photoReportRepository = photoReportRepository;
    }

    public async Task<CollectionResult<ReportDto>> GetResultAsync(long userId)
    {
        ReportDto[] reports;
        reports = await _reportRepository.GetAll()
         .Where(x => x.UserId == userId)
         .Include(x => x.Photos)
         .Select(x => new ReportDto(x.Id, x.Name, x.Description, x.Photos.ToArray(), x.CreatedAt.ToLongDateString()))
         .ToArrayAsync();

        if (!reports.Any())
        {
            _logger.LogWarning(ErrorMessage.ReportsNotFound, reports.Length);

            return new CollectionResult<ReportDto>()
            {
                ErrorMessage = ErrorMessage.ReportsNotFound,
                ErrorCode = (int)ErrorCodes.ReportsNotFound
            };
        }

        return new CollectionResult<ReportDto>
        {
            Data = reports,
            Count = reports.Length
        };
    }

    public async Task<BaseResult<ReportDto>> GetReportAsync(long id)
    {
        try
        {
            var report = await _reportRepository.GetAll()
            .Where(x => x.Id == id)
            .Select(x => new ReportDto(x.Id, x.Name, x.Description, x.Photos.ToArray(), x.CreatedAt.ToLongDateString()))
            .FirstOrDefaultAsync();

            if (report == null)
            {
                _logger.LogWarning($"Отчёт с таким {id} не найден", id);

                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessage.ReportNotFound,
                    ErrorCode = (int)ErrorCodes.ReportNotFound
                };
            }
            return new BaseResult<ReportDto>
            {
                Data = report
            };
        }
        catch (Exception ex)
        {
            return LogErrorHelper<ReportDto>.LogException(ex.Message, _logger);
        }
    }

    /// <summary>
    /// Создание отчёта.
    /// </summary>
    /// <param name="dto">Дто с информацией для создания отчёта.</param>
    /// <returns>Дто созданного отчёта.</returns>
    public async Task<BaseResult<ReportDto>> CreateReportAsync(CreateReportDto dto)
    {
        try
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.UserId);
            var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name);
            var result = _reportValidator.CreateReportValidator(report, user);

            if (!result.IsSuccess)
            {
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = result.ErrorMessage,
                    ErrorCode = result.ErrorCode
                };
            }

            report = new Report()
            {
                Name = dto.Name,
                Description = dto.Description,
                UserId = dto.UserId,
                CreatedAt = DateTime.UtcNow
            };
            if (dto.Photos.IsNotNullOrEmpty())
            {
                report.Photos = dto.Photos
                    .Where(p => !string.IsNullOrWhiteSpace(p.Key))
                    .Select(p => new ReportPhoto
                    {
                        Content = Convert.FromBase64String(p.Key),
                        Description = string.IsNullOrWhiteSpace(p.Value) ? String.Empty : p.Value
                    })
                    .ToList();
            }

            await _reportRepository.CreateAsync(report);
            await _reportRepository.SaveChangeAsync();

            return new BaseResult<ReportDto>()
            {
                Data = _mappper.Map<ReportDto>(report)

            };

        }
        catch (Exception ex)
        {
            return LogErrorHelper<ReportDto>.LogException(ex.Message, _logger);
        }
    }

    /// <summary>
    /// Эндпоинт удаления отчёта
    /// </summary>
    /// <param name="id">Id отчёта</param>
    /// <returns>Дто удаленного отчёта</returns>
    public async Task<BaseResult<ReportDto>> DeleteReportAsync(long id)
    {
        try
        {
            var report = await _reportRepository.GetAll().
        FirstOrDefaultAsync(x => x.Id == id);
            var result = _reportValidator.ValidateOnNull(report);

            if (!result.IsSuccess)
            {
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = result.ErrorMessage,
                    ErrorCode = result.ErrorCode
                };
            }
            _reportRepository.Remove(report);
            await _reportRepository.SaveChangeAsync();

            return new BaseResult<ReportDto>()
            {
                Data = _mappper.Map<ReportDto>(report)
            };
        }
        catch (Exception ex)
        {
            return LogErrorHelper<ReportDto>.LogException(ex.Message, _logger);
        }
    }

    /// <summary>
    /// Обновление отчёта.
    /// </summary>
    /// <param name="dto"> Дто с обновлениями.</param>
    /// <returns>Дто обновленного отчёта.</returns>
    public async Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto dto)
    {
        try
        {
            var report = await _reportRepository.GetAll().
                FirstOrDefaultAsync(x => x.Id == dto.Id);

            var result = _reportValidator.ValidateOnNull(report);

            if (!result.IsSuccess)
            {
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = result.ErrorMessage,
                    ErrorCode = result.ErrorCode
                };
            }

            report.Name = dto.Name;
            report.Description = dto.Description;

            if (dto.Photos is null || dto.Photos.Count <= 0)
            {
                report.Photos = null;
                await _photoReportRepository.GetAll()
                .Where(photo => photo.ReportId == dto.Id)
                .ExecuteDeleteAsync();
            }
            else
            {
                report.Photos = report.Photos = dto.Photos.Count >= 1 ? dto.Photos
                    .Where(p => !string.IsNullOrWhiteSpace(p.Key))
                    .Select(p => new ReportPhoto
                    {
                        Content = Convert.FromBase64String(p.Key),
                        Description = string.IsNullOrWhiteSpace(p.Value) ? String.Empty : p.Value
                    })
                    .ToList() : null;
            }
       
            var updatedReport = _reportRepository.Update(report);
            await _reportRepository.SaveChangeAsync();

            return new BaseResult<ReportDto>()
            {
                Data = _mappper.Map<ReportDto>(updatedReport)
            };
        }
        catch (Exception ex)
        {
            return LogErrorHelper<ReportDto>.LogException(ex.Message, _logger);
        }
    }
}