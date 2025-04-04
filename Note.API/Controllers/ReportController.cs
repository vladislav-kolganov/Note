using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Note.Domain.Dto.ReportDto;
using Note.Domain.Interfaces.Services;
using Note.Domain.Result;

namespace Note.API.Controllers
{
    //[Authorize]
    [ApiVersion("1.0")]
    [Route("[controller]")] // путь до контроллера
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Получение отчёта по Id
        /// </summary>
        /// <param name="id">Id отчёта</param>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ReportDto>>> GetReport(long id)
        {
            var response = await _reportService.GetReportAsync(id);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Получение получения отчётов пользователя по Id пользователя
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        [HttpGet("reports/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CollectionResult<ReportDto>>> GetUserReports(long userId)
        {
            var response = await _reportService.GetResultAsync(userId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// Удаление отчёта по
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns></returns>
        [HttpDelete("{reportId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ReportDto>>> DeleteReport(long reportId)
        {
            var response = await _reportService.DeleteReportAsync(reportId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportDto"></param>
        /// <remarks> 
        /// Request for create report
        /// Post { 
        ///       "Name" : "Report test",
        ///       "Description" : "Test description",
        ///       "userid" : 1}
        /// </remarks>



        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]             //FromBody - означает, что считываем с тела запроса
        public async Task<ActionResult<BaseResult<ReportDto>>> CreateReport([FromBody] CreateReportDto reportDto)
        {
            var response = await _reportService.CreateReportAsync(reportDto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

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
    }
}
