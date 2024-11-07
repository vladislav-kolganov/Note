using Microsoft.AspNetCore.Mvc;
using Note.Application.Services;
using Note.Domain.Dto;
using Note.Domain.Dto.ReportDto;
using Note.Domain.Interfaces.Services;
using Note.Domain.Result;

namespace Note.API.Controllers
{/// <summary>
 /// 
 /// </summary>
    [ApiController]
    public class TokenController : Controller
    {
      private readonly ITokenService _tokenService;
        public TokenController(ITokenService tokenService) 
        {
            _tokenService = tokenService;
        }
        [HttpPost]
        [Route("refresh")]
        public async Task<ActionResult<BaseResult<TokenDto>>> RefreshToken([FromBody]TokenDto tokenDto)
        {
            var response = await _tokenService.RefreshToken(tokenDto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
