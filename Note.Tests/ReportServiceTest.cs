using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Note.Application.Services;
using Note.Domain.Dto.ReportDto;
using Note.Domain.Entity;
using Note.Domain.Enum;
using Note.Domain.Interfaces.Database;
using Note.Domain.Interfaces.Repositories;
using Note.Domain.Interfaces.Validations;
using Note.Domain.Result;
using Note.Tests.Configurations;
using Xunit;

namespace Note.Tests;
public class ReportServiceTest
{
    private readonly IMapper _mapper = MapperConfigurations.GetMapperConfiguration();

    private ReportService CreateReportService(
        Mock<IBaseRepository<Report>>? reportRepo = null,
        Mock<IBaseRepository<User>>? userRepo = null,
        Mock<IBaseRepository<ReportPhoto>>? photoRepo = null,
        Mock<IBaseRepository<UserReport>>? userReportRepo = null,
        Mock<IBaseRepository<Note.Domain.Entity.Map.ReportMapMarker>>? mapMarkerRepo = null,
        Mock<IBaseRepository<Note.Domain.Entity.Map.ReportMapMarkerAttachment>>? attachmentRepo = null,
        Mock<ICastomValidator>? validator = null,
        Mock<IUnitOfWork>? unitOfWork = null,
        Mock<ILogger<ReportService>>? logger = null)
    {
        var mockTransaction = new Mock<IDbContextTransaction>();
        mockTransaction.Setup(t => t.CommitAsync(default)).Returns(Task.CompletedTask);
        mockTransaction.Setup(t => t.RollbackAsync(default)).Returns(Task.CompletedTask);

        var uow = unitOfWork ?? new Mock<IUnitOfWork>();
        uow.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
        uow.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        var val = validator ?? new Mock<ICastomValidator>();
        val.Setup(v => v.ValidateReportOnNull(It.IsAny<Report>())).Returns(new BaseResult());
        val.Setup(v => v.ValidateReportOnNull(null!)).Returns(new BaseResult
        {
            ErrorMessage = "ReportNotFound",
            ErrorCode = (int)ErrorCodes.ReportNotFound
        });
        val.Setup(v => v.UserExistValidator(It.IsAny<User>())).Returns(new BaseResult());
        val.Setup(v => v.UserExistValidator(null!)).Returns(new BaseResult
        {
            ErrorMessage = "UserNotFound",
            ErrorCode = (int)ErrorCodes.UserNotFound
        });

        return new ReportService(
            reportRepo?.Object ?? MockRepositoriesGetter.GetMockReportRepository().Object,
            userRepo?.Object ?? MockRepositoriesGetter.GetMockUserRepository().Object,
            photoRepo?.Object ?? MockRepositoriesGetter.GetMockReportPhotoRepository().Object,
            userReportRepo?.Object ?? MockRepositoriesGetter.GetMockUserReportRepository().Object,
            mapMarkerRepo?.Object ?? MockRepositoriesGetter.GetMockReportMapMarkerRepository().Object,
            attachmentRepo?.Object ?? MockRepositoriesGetter.GetMockReportMapMarkerAttachmentRepository().Object,
            val.Object,
            logger?.Object ?? new Mock<ILogger<ReportService>>().Object,
            uow.Object,
            _mapper
        );
    }

    [Fact]
    public async Task GetSharedReportAsync_ShouldReturnReports_WhenUserHasSharedReports()
    {
        // Arrange
        var service = CreateReportService();

        // Act — User 2 has a shared report (OwnerId=1, UserId=2, ReportId=2)
        var result = await service.GetSharedReportAsync(userId: 2);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetUserReportsAsync_ShouldReturnReports_WhenUserOwnsReports()
    {
        // Arrange
        var service = CreateReportService();

        // Act
        var result = await service.GetUserReportsAsync(userId: 1);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetReportById_ShouldReturnReport_WhenReportExists()
    {
        // Arrange
        var service = CreateReportService();

        // Act
        var result = await service.GetReportById(reportId: 1);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.Id);
    }

    [Fact]
    public async Task CreateReportAsync_ShouldReturnNewReport_WhenUserExists()
    {
        // Arrange — mock report repo that assigns Id=1 on create so re-query finds it
        var reports = MockRepositoriesGetter.GetReports().ToList();
        var mockReportRepo = new Mock<IBaseRepository<Report>>();
        var mockDbSet = reports.BuildMockDbSet();
        mockReportRepo.Setup(r => r.GetAll()).Returns(() => mockDbSet.Object);
        mockReportRepo.Setup(r => r.CreateAsync(It.IsAny<Report>())).ReturnsAsync((Report e) =>
        {
            e.Id = 1;
            return e;
        });
        mockReportRepo.Setup(r => r.SaveChangeAsync()).ReturnsAsync(1);

        var service = CreateReportService(reportRepo: mockReportRepo);
        var dto = new CreateReportDto(
            Name: "Новый отчёт",
            Description: "Описание нового отчёта",
            UserId: 1,
            Photos: null,
            MapMarkers: null
        );

        // Act
        var result = await service.CreateReportAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task CreateReportAsync_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange — validator returns error for null user (empty user repo → user not found)
        var mockValidator = new Mock<ICastomValidator>();
        mockValidator.Setup(v => v.UserExistValidator(It.IsAny<User>())).Returns(new BaseResult
        {
            ErrorMessage = "UserNotFound",
            ErrorCode = (int)ErrorCodes.UserNotFound
        });

        var service = CreateReportService(validator: mockValidator);
        var dto = new CreateReportDto("Тест", "Описание", UserId: 999, null, null);

        // Act
        var result = await service.CreateReportAsync(dto);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteReportAsync_ShouldReturnTrue_WhenUserReportLinkExists()
    {
        // Arrange
        var service = CreateReportService();

        // Act
        var result = await service.DeleteReportAsync(reportId: 1, userId: 1);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task DeleteReportAsync_ShouldReturnError_WhenUserReportLinkNotFound()
    {
        // Arrange
        var mockUserReportRepo = new Mock<IBaseRepository<UserReport>>();
        var emptyData = new List<UserReport>().AsQueryable().BuildMockDbSet();
        mockUserReportRepo.Setup(r => r.GetAll()).Returns(() => emptyData.Object);

        var service = CreateReportService(userReportRepo: mockUserReportRepo);

        // Act
        var result = await service.DeleteReportAsync(reportId: 999, userId: 999);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task UpdateReportAsync_ShouldReturnUpdatedReport_WhenReportIsShared()
    {
        // Arrange — Report 2 is shared (UserReport entries: OwnerId=1/UserId=1 and OwnerId=1/UserId=2)
        // The "shared with others" branch creates a new report
        var reports = MockRepositoriesGetter.GetReports().ToList();
        var mockReportRepo = new Mock<IBaseRepository<Report>>();
        var mockDbSet = reports.BuildMockDbSet();
        mockReportRepo.Setup(r => r.GetAll()).Returns(() => mockDbSet.Object);
        mockReportRepo.Setup(r => r.CreateAsync(It.IsAny<Report>())).ReturnsAsync((Report e) =>
        {
            e.Id = 1;
            return e;
        });
        mockReportRepo.Setup(r => r.SaveChangeAsync()).ReturnsAsync(1);

        var service = CreateReportService(reportRepo: mockReportRepo);
        var dto = new UpdateReportDto(
            reportId: 2,
            userId: 1,
            Name: "Обновлённый отчёт",
            Description: "Новое описание",
            Photos: null,
            MapMarkers: null
        );

        // Act
        var result = await service.UpdateReportAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ShareReport_ShouldReturnTrue_WhenOwnerSharesWithAnotherUser()
    {
        // Arrange — User 2 is not yet linked to Report 1
        var userReports = new List<UserReport>
        {
            new() { UserId = 1, OwnerId = 1, ReportId = 1, IsDeleteForThisUser = false }
        }.AsQueryable();

        var mockUserReportRepo = new Mock<IBaseRepository<UserReport>>();
        var mockDbSet = userReports.BuildMockDbSet();
        mockUserReportRepo.Setup(r => r.GetAll()).Returns(() => mockDbSet.Object);
        mockUserReportRepo.Setup(r => r.CreateAsync(It.IsAny<UserReport>())).ReturnsAsync((UserReport e) => e);
        mockUserReportRepo.Setup(r => r.SaveChangeAsync()).ReturnsAsync(1);

        var service = CreateReportService(userReportRepo: mockUserReportRepo);
        var dto = new ShareReportDto(OwnerUserId: 1, TargetUserId: 2, ReportId: 1);

        // Act
        var result = await service.ShareReport(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Data);
    }
}