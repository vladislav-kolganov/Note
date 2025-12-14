using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Note.Domain.Dto.ReportDto;
using Note.Domain.Interfaces.Services;
using Note.Domain.Result;

namespace Note.API.Controllers;

//[Authorize]
[AllowAnonymous]
[ApiVersion("1.0")]
[Route("[controller]")]
[ApiController]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Получение отчёта по Id.
    /// </summary>
    /// <param name="id">Id отчёта.</param>
    /// <returns>Дто отчёта.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<ReportDto>>> GetReport(long id)
    {
        var response = await _reportService.GetReportById(id);
        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Получение получения отчётов пользователя по Id пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Дто отчёта.</returns>
    [HttpGet("reports/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CollectionResult<ReportDto>>> GetUserReports(long userId)
    {
        var response = await _reportService.GetUserReportsAsync(userId);
        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Получение отчётов, которыми поделились с пользователем.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    [HttpGet("shared-reports/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CollectionResult<ReportDto>>> GetSharedReports(long userId)
    {
        var response = await _reportService.GetSharedReportAsync(userId);
        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Удаление отчёта по Id.
    /// </summary>
    /// <param name="reportId">Id отчёта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>True, если получилось удалить отчёт, иначе false.</returns>
    [HttpDelete("delete-report")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<bool>>> DeleteReport(long reportId, long userId)
    {
        var response = await _reportService.DeleteReportAsync(reportId, userId);
        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Создать отчёт.
    /// </summary>
    /// <param name="reportDto">Дто создания отчёта.</param>
    /// <remarks> 
    /// Request for create report
    /// Post { 
    ///       "Name" : "Report test",
    ///       "Description" : "Test description",
    ///       "userid" : 1}
    /// </remarks>
    /// <returns>Дто отчёта.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<ReportDto>>> CreateReport([FromBody] CreateReportDto reportDto)
    {
        var response = await _reportService.CreateReportAsync(reportDto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Обновить отчёт.
    /// </summary>
    /// <param name="reportDto">Дто обновления отчёта.</param>
    /// <returns>Дто отчёта.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<ReportDto>>> UpdateReport([FromBody] UpdateReportDto reportDto)
    {
        var response = await _reportService.UpdateReportAsync(reportDto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Поделиться отчётом.
    /// </summary>
    /// <param name="shareReportDto">Дто обновления отчёта.</param>
    /// <returns>True, если получилось поделиться отчётом, иначе false.</returns>
    [HttpPost("share-report")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<bool>>> ShareReport([FromBody] ShareReportDto shareReportDto)
    {
        var response = await _reportService.ShareReport(shareReportDto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}