using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Note.Application.Resources;
using Note.Application.Services.Extensions;
using Note.Application.Services.Helpers;
using Note.Domain.Dto.ReportDto;
using Note.Domain.Entity;
using Note.Domain.Enum;
using Note.Domain.Interfaces.Database;
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
    private readonly IBaseRepository<UserReport> _userReportRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICastomValidator _customValidator;
    private readonly IMapper _mappper;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        IBaseRepository<Report> reportRepository,
        IBaseRepository<User> userRepository,
        IBaseRepository<ReportPhoto> photoReportRepository,
        IBaseRepository<UserReport> userReportRepository,
        ICastomValidator customValidator,
        ILogger<ReportService> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _reportRepository = reportRepository;
        _userRepository = userRepository;
        _photoReportRepository = photoReportRepository;
        _userReportRepository = userReportRepository;
        _unitOfWork = unitOfWork;
        _customValidator = customValidator;
        _logger = logger;
        _mappper = mapper;
    }

    public async Task<CollectionResult<ReportDto>> GetUserReportsAsync(long userId)
    {
        try
        {
            var userReportIds = await _userReportRepository
                .GetAll()
                .Where(l => l.OwnerId == userId && l.UserId == userId
                    && !l.IsDeleteForThisUser).
                    Select(l => l.ReportId)
                .Distinct()
                .ToListAsync();

            if (userReportIds.IsNullOrEmpty())
            {
                return new CollectionResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessage.ReportsNotFound,
                    ErrorCode = (int)ErrorCodes.ReportsNotFound
                };
            }

            var reports = await _reportRepository
                .GetAll()
                .Where(x => userReportIds.Contains(x.Id))
                .Include(x => x.Photos)
                .Select(x => new ReportDto(
                    x.Id, x.Name,
                    x.Description,
                    x.Photos.ToArray(),
                    x.CreatedAt.ToLongDateString()))
                .ToArrayAsync();

            if (reports.IsNullOrEmpty())
            {
                _logger.LogWarning(ErrorMessage.ReportsNotFound);

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
        catch (Exception ex)
        {
            return LogErrorHelper<ReportDto>.LogExceptionForCollection(ex.Message, _logger);
        }
    }

    public async Task<CollectionResult<ReportDto>> GetSharedReportAsync(long userId)
    {
        try
        {
            var userReportIds = await _userReportRepository
                           .GetAll()
                           .Where(l => l.OwnerId != userId && l.UserId == userId
                               && !l.IsDeleteForThisUser).
                               Select(l => l.ReportId)
                           .Distinct()
                           .ToListAsync();

            if (userReportIds.IsNullOrEmpty())
            {
                return new CollectionResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessage.ReportsNotFound,
                    ErrorCode = (int)ErrorCodes.ReportsNotFound
                };
            }

            var reports = await _reportRepository
                .GetAll()
                .Where(x => userReportIds.Contains(x.Id))
                .Include(x => x.Photos)
                .Select(x => new ReportDto(
                    x.Id, x.Name,
                    x.Description,
                    x.Photos.ToArray(),
                    x.CreatedAt.ToLongDateString()))
                .ToArrayAsync();

            if (reports.IsNullOrEmpty())
            {
                _logger.LogWarning(ErrorMessage.ReportsNotFound);

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
        catch (Exception ex)
        {
            return LogErrorHelper<ReportDto>.LogExceptionForCollection(ex.Message, _logger);
        }
    }

    public async Task<BaseResult<bool>> ShareReport(ShareReportDto dto)
    {
        // проверяем не шерит ли юзер сам себе отчёт 
        if (dto.TargetUserId == dto.OwnerUserId)
        {
            return new BaseResult<bool>()
            {
                ErrorMessage = ErrorMessage.YouCantShareTheReportWithYourself,
                ErrorCode = (int)ErrorCodes.YouCantShareTheReportWithYourself
            };
        }

        var links = await _userReportRepository //все записи с отчётом который хотим расшерить
            .GetAll()
            .Where(x => x.ReportId == dto.ReportId)
            .ToArrayAsync();
        if (links.IsNullOrEmpty()) // проверяем что записи с таким отчётом вообще существуют, если нет - выходим, иначе нечего шерить.
        {
            return new BaseResult<bool>()
            {
                ErrorMessage = ErrorMessage.ReportsNotFound,
                ErrorCode = (int)ErrorCodes.ReportsNotFound
            };
        }

        var haveRightsForSharing = links.FirstOrDefault(
            x => x.OwnerId == dto.OwnerUserId
              && x.UserId == dto.OwnerUserId);

        if ( haveRightsForSharing is null)
        {
            return new BaseResult<bool>()
            {
                ErrorMessage = ErrorMessage.YouDontHaveTheRightsToShareThisReport,
                ErrorCode = (int)ErrorCodes.YouDontHaveTheRightsToShareThisReport
            };
        }

        var sharedLink = links.FirstOrDefault(x => x.UserId == dto.TargetUserId); // проверяем был ли ранее этот отчёт расшерен пользователю, должна быть одна запись
        if (sharedLink is null) // если отчёт не был расшерин ранее, то шерим отчёт
        {
            var newLink = new UserReport()
            {
                UserId = dto.TargetUserId,
                OwnerId = dto.OwnerUserId,
                ReportId = dto.ReportId,
                IsDeleteForThisUser = false
            };

            await _userReportRepository.CreateAsync(newLink);
            await _userReportRepository.SaveChangeAsync();

            return new BaseResult<bool>()
            {
                Data = true
            };
        }
        if (!sharedLink.IsDeleteForThisUser) // проверяем кейс, что отчёт был ранее расшерин и пользователь у себя его не удалил
        {
            return new BaseResult<bool>()
            {
                ErrorMessage = ErrorMessage.ReportAlreadyShared,
                ErrorCode = (int)ErrorCodes.ReportAlreadyShared
            };
        }
        else // кейс, что отчёт был ранее расшерин и пользователь у себя его удалил
        {
            sharedLink.IsDeleteForThisUser = false;

            _userReportRepository.Update(sharedLink);
            await _userReportRepository.SaveChangeAsync();

            return new BaseResult<bool>()
            {
                Data = true
            };
        }
    }

    public async Task<BaseResult<ReportDto>> GetReportById(long reportId)
    {
        var report = await _reportRepository.GetAll()
            .Include(r => r.Photos)
            .FirstOrDefaultAsync(r => r.Id == reportId);
        var result = _customValidator.ValidateReportOnNull(report);
        if (!result.IsSuccess)
        {
            return new BaseResult<ReportDto>()
            {
                ErrorMessage = result.ErrorMessage,
                ErrorCode = result.ErrorCode
            };
        }

        return new BaseResult<ReportDto>()
        {
            Data = _mappper.Map<ReportDto>(report)
        };
    }

    public async Task<BaseResult<ReportDto>> CreateReportAsync(CreateReportDto dto)
    {

        var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.UserId);
        var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name);
        var result = _customValidator.UserExistValidator(user);

        if (!result.IsSuccess)
        {
            return new BaseResult<ReportDto>()
            {
                ErrorMessage = result.ErrorMessage,
                ErrorCode = result.ErrorCode
            };
        }
        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                report = new Report()
                {
                    Name = dto.Name,
                    Description = dto.Description,
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

                report = await _reportRepository.CreateAsync(report);
                await _unitOfWork.SaveChangeAsync();

                var userReport = new UserReport()
                {
                    UserId = dto.UserId,
                    OwnerId = dto.UserId,
                    ReportId = report.Id,
                    IsDeleteForThisUser = false
                };

                await _userReportRepository.CreateAsync(userReport);

                await _unitOfWork.SaveChangeAsync();
                await transaction.CommitAsync();

                return new BaseResult<ReportDto>()
                {
                    Data = _mappper.Map<ReportDto>(report)
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return LogErrorHelper<ReportDto>.LogException(ex.Message, _logger);
            }
        }
    }

    public async Task<BaseResult<bool>> DeleteReportAsync(long reportId, long userId)
    {
        try
        {
            var userReportLink = await _userReportRepository
               .GetAll()
               .FirstOrDefaultAsync(x => x.ReportId == reportId && x.UserId == userId);

            if (userReportLink is null)
            {
                return new BaseResult<bool>
                {
                    ErrorMessage = ErrorMessage.ReportNotFound,
                    ErrorCode = (int)ErrorCodes.ReportNotFound
                };
            }

            userReportLink.IsDeleteForThisUser = true;
            _userReportRepository.Update(userReportLink);
            await _userReportRepository.SaveChangeAsync();

            return new BaseResult<bool>()
            {
                Data = true
            };
        }
        catch (Exception ex)
        {
            return LogErrorHelper<bool>.LogException(ex.Message, _logger);
        }
    }

    public async Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto dto)
    {
        try
        {
            var report = await _reportRepository.GetAll().
                Include(r => r.Photos).
                FirstOrDefaultAsync(x => x.Id == dto.reportId);
            var result = _customValidator.ValidateReportOnNull(report);
            if (!result.IsSuccess)
            {
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = result.ErrorMessage,
                    ErrorCode = result.ErrorCode
                };
            }

            var links = await _userReportRepository
                .GetAll()
                .Where(ur => ur.ReportId == dto.reportId && !ur.IsDeleteForThisUser)
                .ToListAsync();

            var canUpdate = links.FirstOrDefault(x => x.OwnerId == dto.userId && x.UserId == dto.userId);
            if (canUpdate is null) 
            {
                return new BaseResult<ReportDto>() 
                {
                    ErrorMessage = ErrorMessage.YouDontHaveTheRrightsToEditThisReport,
                    ErrorCode = (int)ErrorCodes.YouDontHaveTheRrightsToEditThisReport
                };
            }

            var sharedWithOthers = links.Any(ur => ur.UserId != dto.userId);
            if (!sharedWithOthers) // если отчёт не расшерин то можно обновить оригинал
            {
                report.Name = dto.Name;
                report.Description = dto.Description;

                if (dto.Photos is null || dto.Photos.Count <= 0)
                {
                    report.Photos = null;
                    await _photoReportRepository.GetAll()
                    .Where(photo => photo.ReportId == dto.reportId)
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
            else // если уже отчет расшерин, то создаем копию, чтобы у других осталась старая версия.
            {
                var newReport = await CreateReportAsync(new CreateReportDto(
                    dto.Name,
                    dto.Description,
                    dto.userId,
                    dto.Photos));

                if (!newReport.IsSuccess)
                {
                    return new BaseResult<ReportDto>
                    {
                        ErrorMessage = newReport.ErrorMessage,
                        ErrorCode = newReport.ErrorCode
                    };
                }

                var ownerLink = links.FirstOrDefault(x => x.OwnerId == dto.userId && x.UserId == dto.userId);

                ownerLink.IsDeleteForThisUser = true;
                _userReportRepository.Update(ownerLink);
                await _userReportRepository.SaveChangeAsync();

                return new BaseResult<ReportDto>()
                {
                    Data = newReport.Data
                };
            }
        }
        catch (Exception ex)
        {
            return LogErrorHelper<ReportDto>.LogException(ex.Message, _logger);
        }
    }
}