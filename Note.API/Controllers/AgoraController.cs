using Microsoft.AspNetCore.Mvc;
using Note.Domain.Dto.ChatDto.Agora;

namespace Note.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AgoraController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AgoraController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Эндпоинт, который отдаёт AppId для Agora.
    /// </summary>
    [HttpGet("config")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<AgoraConfigDto> GetConfig()
    {
        var appId = _configuration["Agora:AppId"];

        if (string.IsNullOrWhiteSpace(appId))
        {
            return BadRequest();
        }

        return Ok(new AgoraConfigDto(appId));
    }
}