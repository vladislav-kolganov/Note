using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Note.Domain.Dto.UserDto;
using Note.Domain.Interfaces.Services;
using Note.Domain.Result;

namespace Note.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Эндпоинт создания пользователя
    /// </summary>
    /// <param name="model">Модель пользователя</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<UserDto>>> Create([FromBody] RegisterUserDto model)
    {
        var response = await _userService.CreateAsync(model);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Эндпоинт обновления/удаления у пользователя фотографии.
    /// </summary>
    /// <param name="model">Модель UpdateOrDeletePhotoDto.</param>
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<UserDto>>> UpdateOrDeletePhotoAsync(UpdateOrDeletePhotoDto model)
    {
        var response = await _userService.UpdateOrDeletePhotoAsync(model);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Эндпоинт удаления пользователя
    /// </summary>
    /// <param name="id"> Id пользователя</param>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<UserDto>>> Delete(long id)
    {
        var response = await _userService.DeleteAsync(id);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Эндпоинт поиска пользователей по логину.
    /// </summary>
    /// <param name="login"> Логин пользователя.</param>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<UserDto>>> FindUserAsync(string login)
    {
        var response = await _userService.FindUsersAsync(login);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Эндпоинт поиска пользователей по логину.
    /// </summary>>
    /// <param name="id"> Id пользователя.</param>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<UserDto>>> GetUserByIdAsync(long id)
    {
        var response = await _userService.GetUserByIdAsync(id);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}