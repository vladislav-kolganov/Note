using Microsoft.AspNetCore.Mvc;
using Note.Domain.Dto;
using Note.Domain.Dto.UserDto;
using Note.Domain.Interfaces.Services;
using Note.Domain.Result;

namespace Note.API.Controllers
{
    /// <summary>
    /// Контроллер для регистрации и логина
    /// </summary>
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Контроллер регистрации пользователя
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async  Task<ActionResult<BaseResult<UserDto>>> Register([FromBody]RegisterUserDto dto)
        {
            var response = await _authService.Register(dto);
            if (response.IsSuccess)
            { 
            return Ok(response);
            }
            return BadRequest(response);
        }
        /// <summary>
        /// Контроллер для логина пользователя
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<BaseResult<TokenDto>>> Login([FromBody] LoginUserDto dto)
        {
            var response = await _authService.Login(dto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

    }
}
