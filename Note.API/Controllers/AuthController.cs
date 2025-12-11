using Microsoft.AspNetCore.Mvc;
using Note.Domain.Dto;
using Note.Domain.Dto.UserDto;
using Note.Domain.Interfaces.Services;
using Note.Domain.Result;

namespace Note.API.Controllers;

/// <summary>
/// Контроллер для регистрации, логина и смены пароля.
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
    /// Эндпоинт регистрации пользователя
    /// </summary>
    /// <param name="dto">dto регистрации юзера</param>
    [HttpPost("Register")]
    public async Task<ActionResult<BaseResult<UserDto>>> Register([FromBody] RegisterUserDto dto)
    {
        var response = await _authService.Register(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Эндпоинт для логина пользователя
    /// </summary>
    /// <param name="dto">dto логина юзера</param>
    [HttpPost("Login")]
    public async Task<ActionResult<BaseResult<TokenDto>>> Login([FromBody] LoginUserDto dto)
    {
        var response = await _authService.Login(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Эндпоинт для смены пароля.
    /// </summary>
    /// <param name="dto">dto логина юзера</param>
    [HttpPost("Password")]
    public async Task<ActionResult<BaseResult<TokenDto>>> ResetPassword([FromBody] ResetPasswordUserDto dto)
    {
        var response = await _authService.ResetPasswordAsync(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}
