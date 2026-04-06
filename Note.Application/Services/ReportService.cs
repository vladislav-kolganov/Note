using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Note.Application.Resources;
using Note.Application.Services.Extensions;
using Note.Application.Services.Helpers;
using Note.Domain.Dto.ReportDto;
using Note.Domain.Entity;
using Note.Domain.Entity.Map;
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
    private readonly IBaseRepository<ReportMapMarker> _reportMapMarkerRepository;
    private readonly IBaseRepository<ReportMapMarkerAttachment> _reportMapMarkerAttachmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICastomValidator _customValidator;
    private readonly IMapper _mappper;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        IBaseRepository<Report> reportRepository,
        IBaseRepository<User> userRepository,
        IBaseRepository<ReportPhoto> photoReportRepository,
        IBaseRepository<UserReport> userReportRepository,
        IBaseRepository<ReportMapMarker> reportMapMarkerRepository,
        IBaseRepository<ReportMapMarkerAttachment> reportMapMarkerAttachmentRepository,
        ICastomValidator customValidator,
        ILogger<ReportService> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _reportRepository = reportRepository;
        _userRepository = userRepository;
        _photoReportRepository = photoReportRepository;
        _userReportRepository = userReportRepository;
        _reportMapMarkerRepository = reportMapMarkerRepository;
        _reportMapMarkerAttachmentRepository = reportMapMarkerAttachmentRepository;
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
                .Where(l => l.OwnerId == userId && l.UserId == userId && !l.IsDeleteForThisUser)
                .Select(l => l.ReportId)
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
                .Include(x => x.MapMarkers)
                    .ThenInclude(m => m.Attachments)
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
                Data = _mappper.Map<ReportDto[]>(reports),
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
                .Where(l => l.OwnerId != userId && l.UserId == userId && !l.IsDeleteForThisUser)
                .Select(l => l.ReportId)
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
                .Include(x => x.MapMarkers)
                    .ThenInclude(m => m.Attachments)
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
                Data = _mappper.Map<ReportDto[]>(reports),
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
        if (dto.TargetUserId == dto.OwnerUserId)
        {
            return new BaseResult<bool>()
            {
                ErrorMessage = ErrorMessage.YouCantShareTheReportWithYourself,
                ErrorCode = (int)ErrorCodes.YouCantShareTheReportWithYourself
            };
        }

        var links = await _userReportRepository
            .GetAll()
            .Where(x => x.ReportId == dto.ReportId)
            .ToArrayAsync();

        if (links.IsNullOrEmpty())
        {
            return new BaseResult<bool>()
            {
                ErrorMessage = ErrorMessage.ReportsNotFound,
                ErrorCode = (int)ErrorCodes.ReportsNotFound
            };
        }

        var haveRightsForSharing = links.FirstOrDefault(
            x => x.OwnerId == dto.OwnerUserId && x.UserId == dto.OwnerUserId);

        if (haveRightsForSharing is null)
        {
            return new BaseResult<bool>()
            {
                ErrorMessage = ErrorMessage.YouDontHaveTheRightsToShareThisReport,
                ErrorCode = (int)ErrorCodes.YouDontHaveTheRightsToShareThisReport
            };
        }

        var sharedLink = links.FirstOrDefault(x => x.UserId == dto.TargetUserId);

        if (sharedLink is null)
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

        if (!sharedLink.IsDeleteForThisUser)
        {
            return new BaseResult<bool>()
            {
                ErrorMessage = ErrorMessage.ReportAlreadyShared,
                ErrorCode = (int)ErrorCodes.ReportAlreadyShared
            };
        }

        sharedLink.IsDeleteForThisUser = false;
        _userReportRepository.Update(sharedLink);
        await _userReportRepository.SaveChangeAsync();

        return new BaseResult<bool>()
        {
            Data = true
        };
    }

    public async Task<BaseResult<ReportDto>> GetReportById(long reportId)
    {
        try
        {
            var report = await _reportRepository.GetAll()
                .Include(r => r.Photos)
                .Include(r => r.MapMarkers)
                    .ThenInclude(m => m.Attachments)
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
        catch (Exception ex)
        {
            return LogErrorHelper<ReportDto>.LogException(ex.Message, _logger);
        }
    }

    public async Task<BaseResult<ReportDto>> CreateReportAsync(CreateReportDto dto)
    {
        var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.UserId);
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
                var report = new Report()
                {
                    Name = dto.Name.Trim(),
                    Description = dto.Description,
                    CreatedAt = DateTime.UtcNow,
                    Photos = MapFilesHelper.BuildReportPhotos(dto.Photos) ?? new List<ReportPhoto>(),
                    MapMarkers = MapFilesHelper.BuildMapMarkers(dto.MapMarkers) ?? new List<ReportMapMarker>()
                };

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

                var createdReport = await _reportRepository.GetAll()
                    .Include(r => r.Photos)
                    .Include(r => r.MapMarkers)
                        .ThenInclude(m => m.Attachments)
                    .FirstOrDefaultAsync(r => r.Id == report.Id);

                return new BaseResult<ReportDto>()
                {
                    Data = _mappper.Map<ReportDto>(createdReport)
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
            var report = await _reportRepository.GetAll()
                .Include(r => r.Photos)
                .Include(r => r.MapMarkers)
                    .ThenInclude(m => m.Attachments)
                .FirstOrDefaultAsync(x => x.Id == dto.reportId);

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

            if (!sharedWithOthers)
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        report.Name = dto.Name.Trim();
                        report.Description = dto.Description;
                        report.UpdatedAt = DateTime.UtcNow;

                        // 1. Обновляем обычные фото отчёта
                        await ReplaceReportPhotosAsync(dto.reportId, report, dto.Photos);

                        // 2. Обновляем карту (полная замена маркеров)
                        await ReplaceMapMarkersAsync(dto.reportId, dto.MapMarkers);

                        _reportRepository.Update(report);
                        await _unitOfWork.SaveChangeAsync();

                        await transaction.CommitAsync();

                        var updatedReport = await _reportRepository.GetAll()
                            .Include(r => r.Photos)
                            .Include(r => r.MapMarkers)
                                .ThenInclude(m => m.Attachments)
                            .FirstOrDefaultAsync(r => r.Id == dto.reportId);

                        return new BaseResult<ReportDto>()
                        {
                            Data = _mappper.Map<ReportDto>(updatedReport)
                        };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return LogErrorHelper<ReportDto>.LogException(ex.Message, _logger);
                    }
                }
            }
            else
            {
                var newReport = await CreateReportAsync(new CreateReportDto(
                    dto.Name,
                    dto.Description,
                    dto.userId,
                    dto.Photos,
                    dto.MapMarkers));

                if (!newReport.IsSuccess)
                {
                    return new BaseResult<ReportDto>
                    {
                        ErrorMessage = newReport.ErrorMessage,
                        ErrorCode = newReport.ErrorCode
                    };
                }

                var ownerLink = links.FirstOrDefault(x => x.OwnerId == dto.userId && x.UserId == dto.userId);

                ownerLink!.IsDeleteForThisUser = true;
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

    #region Private Helpers

    private async Task ReplaceReportPhotosAsync(long reportId, Report report, Dictionary<string, string>? photos)
    {
        await _photoReportRepository.GetAll()
            .Where(photo => photo.ReportId == reportId)
            .ExecuteDeleteAsync();

        report.Photos = MapFilesHelper.BuildReportPhotos(photos) ?? new List<ReportPhoto>();
    }

    private async Task ReplaceMapMarkersAsync(long reportId, List<CreateReportMapMarkerDto>? mapMarkers)
    {
        await _reportMapMarkerRepository.GetAll()
            .Where(marker => marker.ReportId == reportId)
            .ExecuteDeleteAsync();

        var newMarkers = MapFilesHelper.BuildMapMarkers(mapMarkers);
        if (newMarkers.IsNullOrEmpty())
            return;

        foreach (var marker in newMarkers)
        {
            marker.ReportId = reportId;
            await _reportMapMarkerRepository.CreateAsync(marker);
        }
    }
    #endregion
}