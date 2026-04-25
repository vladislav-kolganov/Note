using Microsoft.AspNetCore.Mvc;
using Note.Domain.Dto.DiagramDto;
using Note.Domain.Interfaces.Services;
using Note.Domain.Result;

namespace Note.API.Controllers;

[ApiController]
public class DiagramController : Controller
{
    private readonly IDiagramService _diagramService;

    public DiagramController(IDiagramService diagramService)
    {
        _diagramService = diagramService;
    }

    /// <summary>
    /// Эндпоинт получения данных для Grouped Bar Chart.
    /// По количеству меток пожара.
    /// </summary>
    /// <param name="reportIds">Id отчётов.</param>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CollectionResult<FireClassChartItemDto>>> GetFireClassChart(long[] reportIds)
    {
        var response = await _diagramService.GetFireClassChartAsync(reportIds);

        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }

        return BadRequest(response);
    }
}