using Microsoft.AspNetCore.Mvc;
using Note.Domain.Entity.ChatEntity;
using Note.Domain.Interfaces.Services;
using Note.Domain.Result;

namespace Note.API.Controllers
{
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
        /// Метод получения чатов
        /// </summary>
        /// <param name="userId">Id юзера</param>
        [HttpGet(nameof(GetChats) + "/" + nameof(userId))]
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
        /// Метод получения последнего сообщения
        /// </summary>
        /// <param name="chatId"> Id чата</param>
        [HttpGet($"{nameof(GetLastMessage)}/{{{nameof(chatId)}}}")]
        public async Task<ActionResult<BaseResult<Chat>>> GetLastMessage(long chatId)
        {
            var response = await _chatService.GetLastMessage(chatId);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        /// <summary>
        /// Метод получения сообщения
        /// </summary>
        /// <param name="chatId"> Id чата</param>
        [HttpGet(nameof(GetMessages) + "/" + nameof(chatId))]
        public async Task<ActionResult<CollectionResult<Chat>>> GetMessages(long chatId)
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
        /// Метод удаления чата у пользователя
        /// </summary>
        /// <param name="chatId"></param>
        [HttpPost(nameof(DeleteChat))]
        public async Task<ActionResult<BaseResult<Message>>> DeleteChat(long chatId)
        {
            var response = await _chatService.DeleteChat(chatId);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
    
    }
}
