using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Note.Application.Resources;
using Note.Domain.Dto.ReportDto;
using Note.Domain.Entity;
using Note.Domain.Enum;
using Note.Domain.Interfaces.Repositories;
using Note.Domain.Interfaces.Services;
using Note.Domain.Interfaces.Validations;
using Note.Domain.Result;
using Serilog;

namespace Note.Application.Services
{
    public class ReportService : IReportService
    {
        // Сервисы - прослойка между API и DB
        private readonly IBaseRepository<Report> _reportRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IReportValidator _reportValidator;
        private readonly IMapper _mappper;
        private readonly ILogger _logger;

        public ReportService(IBaseRepository<Report> reportRepository, IBaseRepository<User> userRepository, IReportValidator reportValidator, ILogger logger, IMapper mapper)
        {
            _reportRepository = reportRepository;
            _userRepository = userRepository;
            _reportValidator = reportValidator;
            _logger = logger;
            _mappper = mapper;
        }

        public async Task<CollectionResult<ReportDto>> GetResultAsync(long userId)
        {
            ReportDto[] reports;
            try
            {
                reports = await _reportRepository.GetAll()
                    .Where(x => x.UserId == userId)
                    .Select(x => new ReportDto(x.Id, x.Name, x.Description, x.CreatedAt.ToLongDateString()))
                    .ToArrayAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new CollectionResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessage.InternalServerError,
                    ErrorCode = (int)ErrorCodes.InternalServerError
                };
            }

            if (!reports.Any())
            {
                _logger.Warning(ErrorMessage.ReportsNotFound, reports.Length);
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
            ReportDto? report;
            try
            {
                report = await _reportRepository.GetAll()
                    .Where(x => x.Id == id)
                    .Select(x => new ReportDto(x.Id, x.Name, x.Description, x.CreatedAt.ToLongDateString()))
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessage.InternalServerError,
                    ErrorCode = (int)ErrorCodes.InternalServerError
                };

            }
            if (report == null)
            {
                _logger.Warning($"Отчёт с таким {id} не найден", id);
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
                    UserId = dto.UserId
                };
                await _reportRepository.CreateAsync(report);

                return new BaseResult<ReportDto>()
                {
                    Data = _mappper.Map<ReportDto>(report)

                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessage.InternalServerError,
                    ErrorCode = (int)ErrorCodes.InternalServerError
                };

            }
        }

        public async Task<BaseResult<ReportDto>> DeleteReportAsync(long id)
        {
            try
            {
                var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
                var result =  _reportValidator.ValidateOnNull(report);
                if (!result.IsSuccess)
                {
                    return new BaseResult<ReportDto>()
                    {
                        ErrorMessage = result.ErrorMessage,
                        ErrorCode = result.ErrorCode
                    };
                }
                await _reportRepository.RemoveAsync(report);
                return new BaseResult<ReportDto>()
                {
                    Data = _mappper.Map<ReportDto>(report)
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessage.InternalServerError,
                    ErrorCode = (int)ErrorCodes.InternalServerError
                };

            }
        }

        public async Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto dto)
        {
            try
            {
                var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);
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
                await _reportRepository.UpdateAsync(report);
                return new BaseResult<ReportDto>()
                {
                    Data = _mappper.Map<ReportDto>(report)
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessage.InternalServerError,
                    ErrorCode = (int)ErrorCodes.InternalServerError
                };

            }

        }
    }
}
