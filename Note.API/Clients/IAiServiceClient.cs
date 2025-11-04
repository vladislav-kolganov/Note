using Note.Domain.Dto.AiChatDto;
using Note.Domain.Result;

namespace Note.API.Clients;

/// <summary>
/// Интерфейс клиента для запроса в микросервис с Ламой.
/// </summary>
public interface IAiServiceClient
{
    /// <summary>
    /// Метод запроса в микросервис с Llama.
    /// </summary>
    /// <param name="dto">Дто запроса.</param>
    /// <param name="ct">CancellationToken.</param>
    /// <returns>Дто ответа.</returns>
    Task<BaseResult<ChatResponseDto>> AskLlamaAsync(ChatRequestDto dto, CancellationToken ct = default);
}
