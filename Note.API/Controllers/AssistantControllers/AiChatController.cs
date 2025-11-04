using Microsoft.AspNetCore.Mvc;
using Note.API.Clients;
using Note.Domain.Dto.AiChatDto;

namespace Note.API.Controllers.AssistantControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiChatController : ControllerBase
    {
        private readonly IAiServiceClient _aiService;

        public AiChatController(IAiServiceClient aiService)
        {
            _aiService = aiService;
        }

        /// <summary>
        /// Метод запроса в микросервис с Llama.
        /// </summary>
        /// <param name="dto">Дто запроса к сервису.</param>
        /// <param name="ct">CancellationToken.</param>
        /// <returns>Дто ответа нейронки.</returns>
        [HttpPost("ask-llama")]
        public async Task<ActionResult<ChatResponseDto>> AskLlama([FromBody] ChatRequestDto dto, CancellationToken ct)
        {
            var res = await _aiService.AskLlamaAsync(dto, ct);

            return Ok(res);
        }
    }
}
