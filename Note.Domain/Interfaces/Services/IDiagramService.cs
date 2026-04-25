using Note.Domain.Dto.DiagramDto;
using Note.Domain.Result;

namespace Note.Domain.Interfaces.Services;

public interface IDiagramService
{
    public Task<CollectionResult<FireClassChartItemDto>> GetFireClassChartAsync(long[] reportIds);
}