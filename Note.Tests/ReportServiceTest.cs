
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Note.Application.Services;
using Note.Domain.Dto.ReportDto;
using Note.Tests.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Note.Tests
{
    public class ReportServiceTest
    {
        [Fact]
        public async Task GetReport_ShouldBe_NotNull()
        {
            // Arrange
            var mockReportRepository = MockRepositoriesGetter.GetMockReportRepository();
            var mockDistributedCache = new Mock<IDistributedCache>();
            var reportService = new ReportService(mockReportRepository.Object, null, null, null,
                null, null, null, mockDistributedCache.Object);

            // Act
            var result = await reportService.GetReportAsync(1);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CreateReport_ShouldBe_Return_NewReport()
        {
            // Arrange
            var mockReportRepository = MockRepositoriesGetter.GetMockReportRepository();
            var mockUserRepository = MockRepositoriesGetter.GetMockUserRepository();
            var mockDistributedCache = new Mock<IDistributedCache>();
            var mapper = MapperConfigurations.GetMapperConfiguration();

            var user = MockRepositoriesGetter.GetUsers().FirstOrDefault();
            var createReportDto = new CreateReportDto("Деловой отчет #1", "Пока не придумали", user.Id);

            var reportService = new ReportService(mockReportRepository.Object, mockUserRepository.Object, null, null,
                mapper, null, null, mockDistributedCache.Object);

            // Act
            var result = await reportService.CreateReportAsync(createReportDto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteReport_ShouldBe_Return_TrueSuccess()
        {
            // Arrange
            var mockReportRepository = MockRepositoriesGetter.GetMockReportRepository();
            var mapper = MapperConfigurations.GetMapperConfiguration();
            var report = MockRepositoriesGetter.GetReports().FirstOrDefault();

            // Act
            var reportService = new ReportService(mockReportRepository.Object, null, null, null,
                mapper, null, null, null);
            var result = await reportService.DeleteReportAsync(report.Id);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateReport_ShouldBe_Return_NewData_For_Report()
        {
            // Arrange
            var mockReportRepository = MockRepositoriesGetter.GetMockReportRepository();
            var mapper = MapperConfigurations.GetMapperConfiguration();
            var report = MockRepositoriesGetter.GetReports().FirstOrDefault();
            var updateReportDto = new UpdateReportDto(report.Id, "New name for report", "New description for report");

            // Act
            var reportService = new ReportService(mockReportRepository.Object, null, null, null,
                mapper, null, null, null);
            var result = await reportService.UpdateReportAsync(updateReportDto);

            // Assert
            Assert.True(result.IsSuccess);
        }
    }
}
