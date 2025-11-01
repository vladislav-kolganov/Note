using Microsoft.AspNetCore.Mvc;
using Note.Domain.Dto.ChatDto;
using Note.Domain.Entity.ChatEntity;
using Note.Domain.Interfaces.Services;
using Note.Domain.Result;

namespace Note.API.Controllers;

/// <summary>
/// Контроллер чата
/// </summary>
[ApiController]
[Route("[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    #region Получение чатов и сообщений
    /// <summary>
    /// Метод получения чатов.
    /// </summary>
    /// <param name="userId">Id юзера.</param>
    [HttpGet("GetChats/{userId}")]
    public async Task<ActionResult<CollectionResult<Chat>>> GetChats(long userId)
    {
        var response = await _chatService.GetChats(userId);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Метод получения последнего сообщения.
    /// </summary>
    /// <param name="chatId">Id чата.</param>
    [HttpGet("GetLastMessage/{chatId}")]
    public async Task<ActionResult<BaseResult<Message>>> GetLastMessage(long chatId)
    {
        var response = await _chatService.GetLastMessage(chatId);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Метод получения сообщения.
    /// </summary>
    /// <param name="chatId">Id чата.</param>
    [HttpGet("GetMessages/{chatId}")]
    public async Task<ActionResult<CollectionResult<Message>>> GetMessages(long chatId)
    {
        var response = await _chatService.GetMessages(chatId);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
    #endregion

    /// <summary>
    /// Метод удаления чата у пользователя.
    /// </summary>
    /// <param name="chatId">Id чата.</param>
    [HttpDelete("DeleteChat/{chatId}")]
    public async Task<ActionResult<BaseResult<bool>>> DeleteChat(long chatId)
    {
        var response = await _chatService.DeleteChat(chatId);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Метод создания сообщения.
    /// </summary>
    /// <param name="dto">Дто создания сообщения.</param>
    /// <returns>Созданное сообщение.</returns>
    [HttpPost("CreateMessage")]
    public async Task<ActionResult<BaseResult<Message>>> CreateMessage([FromBody] CreateMessageDto dto)
    {
        var response = await _chatService.CreateMessage(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Метод редактирования сообщения.
    /// </summary>
    /// <param name="dto">Дто редактирования сообщения.</param>
    /// <returns>Отредактированное сообщение.</returns>
    [HttpPost("EditMessage")]
    public async Task<ActionResult<BaseResult<Message>>> EditMessage([FromBody] EditMessageDto dto)
    {
        var response = await _chatService.EditMessage(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}
