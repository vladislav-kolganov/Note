using Microsoft.EntityFrameworkCore;
using Note.Application.Resources;
using Note.Application.Services.Extensions;
using Note.Domain.Dto.DiagramDto;
using Note.Domain.Entity.Map;
using Note.Domain.Enum;
using Note.Domain.Enum.BusinessEnums;
using Note.Domain.Interfaces.Repositories;
using Note.Domain.Interfaces.Services;
using Note.Domain.Result;

namespace Note.Application.Services;

/// <summary>
/// Сервис для формирования данных диаграмм на основе маркеров карты.
/// </summary>
public class DiagramService : IDiagramService
{
    private readonly IBaseRepository<ReportMapMarker> _reportMapMarkerRepository;

    public DiagramService(IBaseRepository<ReportMapMarker> reportMapMarkerRepository)
    {
        _reportMapMarkerRepository = reportMapMarkerRepository;
    }

    /// <summary>
    /// Получает данные для построения диаграммы количества меток пожара
    /// по местности и классу пожара.
    /// </summary>
    /// <param name="reportIds">
    /// Массив идентификаторов отчётов, по которым необходимо получить данные для диаграммы.
    /// </param>
    /// <returns>
    /// Результат выполнения операции, содержащий коллекцию элементов диаграммы.
    /// В каждом элементе указано название местности и количество меток пожара
    /// малого, среднего и большого класса.
    /// </returns>
    public async Task<CollectionResult<FireClassChartItemDto>> GetFireClassChartAsync(long[] reportIds)
    {
        if (reportIds.IsNullOrEmpty())
        {
            return new CollectionResult<FireClassChartItemDto>()
            {
                ErrorMessage = ErrorMessage.InvalidClientRequest,
                ErrorCode = (int)ErrorCodes.InvalidClientRequest
            };
        }

        var markers = await _reportMapMarkerRepository
            .GetAll()
            .AsNoTracking()
            .Where(x => reportIds.Contains(x.ReportId))
            .ToArrayAsync();

        if (markers.IsNullOrEmpty())
        {
            return new CollectionResult<FireClassChartItemDto>
            {
                Data = Array.Empty<FireClassChartItemDto>(),
                Count = 0
            };
        }

        var chartItems = markers
            .GroupBy(x => x.LocationName.Trim())
            .Select(group => new FireClassChartItemDto
            {
                LocationName = group.Key,

                SmallCount = group.Count(marker => marker.FireClass == FireClassEnum.Small),

                MediumCount = group.Count(marker => marker.FireClass == FireClassEnum.Medium),

                LargeCount = group.Count(marker => marker.FireClass == FireClassEnum.Large),

                NoFireCount = group.Count(marker => marker.FireClass == FireClassEnum.None)
            })
            .OrderByDescending(item => item.NoFireCount + item.SmallCount + item.MediumCount + item.LargeCount)
            .ToArray();

        return new CollectionResult<FireClassChartItemDto>
        {
            Data = chartItems,
            Count = chartItems.Length
        };
    }
}