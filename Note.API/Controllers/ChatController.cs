using Microsoft.AspNetCore.Mvc;
using Note.Domain.Dto.ChatDto;
using Note.Domain.Entity.ChatEntity;
using Note.Domain.Interfaces.Services;
using Note.Domain.Result;

namespace Note.API.Controllers;

/// <summary>
/// Контроллер чата.
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
    [HttpGet("get-chats/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CollectionResult<Chat>>> GetChats(long userId)
    {
        var response = await _chatService.GetChatsAsync(userId);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Получить список чатов пользователя с логинами и последними сообщениями.
    /// </summary>
    /// <param name="userId">Id юзера.</param>
    [HttpGet("chat-list/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CollectionResult<ChatListItemDto>>> GetChatList(long userId)
    {
        var response = await _chatService.GetChatListAsync(userId);

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
    [HttpGet("last-message/{chatId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<Message>>> GetLastMessage(long chatId)
    {
        var response = await _chatService.GetLastMessageAsync(chatId);

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
    [HttpGet("messages/{chatId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CollectionResult<Message>>> GetMessages(long chatId)
    {
        var response = await _chatService.GetMessagesAsync(chatId);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
    #endregion

    /// <summary>
    /// Метод поиска или создания чата.
    /// </summary>
    /// <param name="dto">Дто создания чата.</param>
    /// <returns>Найденный или созданный чат.</returns>
    [HttpPost("find-or-create-chat")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<Chat>>> FindOrCreateChat([FromBody] UserCreateChatDto dto)
    {
        var response = await _chatService.FindOrCreateChatAsync(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Метод удаления чата у пользователя.
    /// </summary>
    /// <param name="chatId">Id чата.</param>
    [HttpDelete("{chatId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<bool>>> DeleteChat(long chatId)
    {
        var response = await _chatService.DeleteChatAsync(chatId);

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
    [HttpPost("create-message")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<Message>>> CreateMessage([FromBody] CreateMessageDto dto)
    {
        var response = await _chatService.CreateMessageAsync(dto);

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
    [HttpPut("edit-message")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<Message>>> EditMessage([FromBody] EditMessageDto dto)
    {
        var response = await _chatService.EditMessageAsync(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Метод удаления сообщений у пользователя в чате.
    /// </summary>
    /// <param name="uesrId">Id пользователя.</param>
    /// <param name="messagesIds">Id сообщений.</param>
    [HttpDelete("delete-messages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<bool>>> DeleteMessages(DeleteMessageDto dto)
    {
        var response = await _chatService.DeleteMessagesAsync(dto.UserId, dto.MessageIds);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}