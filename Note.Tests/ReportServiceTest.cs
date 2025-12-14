namespace Note.Tests;

public class ReportServiceTest
{
   // [Fact]
    //public async Task GetReport_ShouldBe_NotNull()
    //{
    //    // Arrange
    //    var mockReportRepository = MockRepositoriesGetter.GetMockReportRepository();
    //    var mockDistributedCache = new Mock<IDistributedCache>();
    //    var reportService = new ReportService
    //        (
    //         reportRepository: mockReportRepository.Object,
    //         userRepository: null,
    //         customValidator: null,
    //         photoReportRepository: null,
    //         logger: null,
    //         mapper: (AutoMapper.IMapper)mockDistributedCache.Object
    //       );

    //    // Act
    //    var result = await reportService.GetSharedReportAsync(1);

    //    // Assert
    //    Assert.NotNull(result);
    //}

    //[Fact]
    //public async Task CreateReport_ShouldBe_Return_NewReport()
    //{
    //    // Arrange
    //    var mockReportRepository = MockRepositoriesGetter.GetMockReportRepository();
    //    var mockUserRepository = MockRepositoriesGetter.GetMockUserRepository();
    //    var mockDistributedCache = new Mock<IDistributedCache>();
    //    var mapper = MapperConfigurations.GetMapperConfiguration();

    //    var user = MockRepositoriesGetter.GetUsers().FirstOrDefault();
    //    var createReportDto = new CreateReportDto("Деловой отчет #1", "Пока не придумали", user.Id, null);

    //    var reportService = new ReportService
    //        (
    //         reportRepository: mockReportRepository.Object,
    //         userRepository: mockUserRepository.Object,
    //         photoReportRepository: null,
    //         customValidator: null,
    //         logger: null,
    //         mapper: (AutoMapper.IMapper)mockDistributedCache.Object
    //       );

    //    // Act
    //    var result = await reportService.CreateReportAsync(createReportDto);

    //    // Assert
    //    Assert.True(result.IsSuccess);
    //}

    //[Fact]
    //public async Task DeleteReport_ShouldBe_Return_TrueSuccess()
    //{
    //    // Arrange
    //    var mockReportRepository = MockRepositoriesGetter.GetMockReportRepository();
    //    var mapper = MapperConfigurations.GetMapperConfiguration();
    //    var report = MockRepositoriesGetter.GetReports().FirstOrDefault();

    //    // Act
    //    var reportService = new ReportService
    //        (
    //         reportRepository: mockReportRepository.Object,
    //         userRepository: null,
    //         photoReportRepository: null,
    //         customValidator: null,
    //         logger: null,
    //         mapper: mapper
    //       );

    //    var result = await reportService.DeleteReportAsync(report.Id);

    //    // Assert
    //    Assert.True(result.IsSuccess);
    //}

    //[Fact]
    //public async Task UpdateReport_ShouldBe_Return_NewData_For_Report()
    //{
    //    // Arrange
    //    var mockReportRepository = MockRepositoriesGetter.GetMockReportRepository();
    //    var mapper = MapperConfigurations.GetMapperConfiguration();
    //    var report = MockRepositoriesGetter.GetReports().FirstOrDefault();
    //    var updateReportDto = new UpdateReportDto(report.Id, "New name for report", "New description for report", null);

    //    // Act
    //    var reportService = new ReportService
    //        (
    //         reportRepository: mockReportRepository.Object,
    //         userRepository: null,
    //         customValidator: null,
    //         photoReportRepository: null,
    //         logger: null,
    //         mapper: mapper
    //       );

    //    var result = await reportService.UpdateReportAsync(updateReportDto);

    //    // Assert
    //    Assert.True(result.IsSuccess);
    //}
}