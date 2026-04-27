using MockQueryable.Moq;
using Moq;
using Note.Application.Services;
using Note.Domain.Entity.Map;
using Note.Domain.Enum.BusinessEnums;
using Note.Domain.Interfaces.Repositories;
using Xunit;

namespace Note.Tests;

public class DiagramServiceTest
{
    private DiagramService CreateDiagramService(
        Mock<IBaseRepository<ReportMapMarker>>? markerRepo = null)
    {
        return new DiagramService(
            markerRepo?.Object ?? GetDefaultMockMarkerRepository().Object
        );
    }

    #region Mock Data

    private static List<ReportMapMarker> GetDefaultMarkers()
    {
        return new List<ReportMapMarker>
        {
            new()
            {
                Id = 1, ReportId = 1, LocationName = "Forest",
                FireClass = FireClassEnum.Small, CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = 2, ReportId = 1, LocationName = "Forest",
                FireClass = FireClassEnum.Medium, CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = 3, ReportId = 1, LocationName = "Swamp",
                FireClass = FireClassEnum.Large, CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = 4, ReportId = 2, LocationName = "Field",
                FireClass = FireClassEnum.Small, CreatedAt = DateTime.UtcNow
            },
        };
    }

    private static Mock<IBaseRepository<ReportMapMarker>> GetDefaultMockMarkerRepository()
    {
        return CreateMockMarkerRepository(GetDefaultMarkers());
    }

    private static Mock<IBaseRepository<ReportMapMarker>> CreateMockMarkerRepository(
        List<ReportMapMarker> markers)
    {
        var mock = new Mock<IBaseRepository<ReportMapMarker>>();
        var mockDbSet = markers.AsQueryable().BuildMockDbSet();
        mock.Setup(r => r.GetAll()).Returns(() => mockDbSet.Object);
        return mock;
    }

    #endregion

    #region GetFireClassChartAsync

    [Fact]
    public async Task GetFireClassChartAsync_ShouldReturnChartData_WhenMarkersExist()
    {
        // Arrange
        var service = CreateDiagramService();

        // Act — report 1 has markers in two locations: "Forest" (2) and "Swamp" (1)
        var result = await service.GetFireClassChartAsync(new long[] { 1 });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetFireClassChartAsync_ShouldGroupByLocationAndCountFireClasses()
    {
        // Arrange
        var markers = new List<ReportMapMarker>
        {
            new() { Id = 1, ReportId = 1, LocationName = "Zone A", FireClass = FireClassEnum.Small },
            new() { Id = 2, ReportId = 1, LocationName = "Zone A", FireClass = FireClassEnum.Small },
            new() { Id = 3, ReportId = 1, LocationName = "Zone A", FireClass = FireClassEnum.Medium },
            new() { Id = 4, ReportId = 1, LocationName = "Zone A", FireClass = FireClassEnum.Large },
            new() { Id = 5, ReportId = 1, LocationName = "Zone A", FireClass = FireClassEnum.Large },
            new() { Id = 6, ReportId = 1, LocationName = "Zone A", FireClass = FireClassEnum.Large },
        };
        var service = CreateDiagramService(CreateMockMarkerRepository(markers));

        // Act
        var result = await service.GetFireClassChartAsync(new long[] { 1 });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        var item = result.Data.First();
        Assert.Equal(2, item.SmallCount);
        Assert.Equal(1, item.MediumCount);
        Assert.Equal(3, item.LargeCount);
        Assert.Equal(6, item.TotalCount);
    }

    [Fact]
    public async Task GetFireClassChartAsync_ShouldBeCaseInsensitive_WhenGroupingByLocation()
    {
        // Arrange — same location with different casing and whitespace
        var markers = new List<ReportMapMarker>
        {
            new() { Id = 1, ReportId = 1, LocationName = "  Forest  ", FireClass = FireClassEnum.Small },
            new() { Id = 2, ReportId = 1, LocationName = "forest", FireClass = FireClassEnum.Medium },
            new() { Id = 3, ReportId = 1, LocationName = "FOREST", FireClass = FireClassEnum.Large },
        };
        var service = CreateDiagramService(CreateMockMarkerRepository(markers));

        // Act
        var result = await service.GetFireClassChartAsync(new long[] { 1 });

        // Assert — all three grouped into one location
        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        var item = result.Data.First();
        Assert.Equal("forest", item.LocationName);
        Assert.Equal(1, item.SmallCount);
        Assert.Equal(1, item.MediumCount);
        Assert.Equal(1, item.LargeCount);
    }

    [Fact]
    public async Task GetFireClassChartAsync_ShouldOrderByTotalCountDescending()
    {
        // Arrange — Zone B has 3 markers, Zone A has 1
        var markers = new List<ReportMapMarker>
        {
            new() { Id = 1, ReportId = 1, LocationName = "Zone A", FireClass = FireClassEnum.Small },
            new() { Id = 2, ReportId = 1, LocationName = "Zone B", FireClass = FireClassEnum.Small },
            new() { Id = 3, ReportId = 1, LocationName = "Zone B", FireClass = FireClassEnum.Medium },
            new() { Id = 4, ReportId = 1, LocationName = "Zone B", FireClass = FireClassEnum.Large },
        };
        var service = CreateDiagramService(CreateMockMarkerRepository(markers));

        // Act
        var result = await service.GetFireClassChartAsync(new long[] { 1 });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Count);
        Assert.Equal("zone b", result.Data.First().LocationName);
        Assert.Equal("zone a", result.Data.Last().LocationName);
    }

    [Fact]
    public async Task GetFireClassChartAsync_ShouldFilterByReportIds()
    {
        // Arrange
        var service = CreateDiagramService();

        // Act — only report 2, which has one marker in "Field"
        var result = await service.GetFireClassChartAsync(new long[] { 2 });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal("field", result.Data.First().LocationName);
    }

    [Fact]
    public async Task GetFireClassChartAsync_ShouldReturnEmpty_WhenNoMatchingMarkers()
    {
        // Arrange
        var service = CreateDiagramService();

        // Act — reportId 999 doesn't exist in mock data
        var result = await service.GetFireClassChartAsync(new long[] { 999 });

        // Assert — IsNullOrEmpty bug (&&): empty non-null array returns false,
        // falls through to GroupBy which produces empty result
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Count);
    }

    [Fact]
    public async Task GetFireClassChartAsync_ShouldHandleMultipleReportIds()
    {
        // Arrange — markers from both report 1 and report 2
        var service = CreateDiagramService();

        // Act
        var result = await service.GetFireClassChartAsync(new long[] { 1, 2 });

        // Assert — all markers from both reports: Forest, Swamp, Field
        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Count);
    }

    #endregion
}