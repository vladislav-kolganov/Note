using Note.Domain.Dto.AiChatDto;
using Note.Domain.Enum;
using Note.Domain.Result;

namespace Note.API.Clients;

/// <summary>
/// Реализация клиента для запроса в другой микросервис с Lama.
/// </summary>
public class AiServiceClient : IAiServiceClient
{
    private readonly HttpClient _http;
    private readonly ILogger<AiServiceClient> _logger;

    public AiServiceClient(HttpClient http, ILogger<AiServiceClient> logger)
    {
        _http = http;
        _logger = logger;
    }
    
    /// <inheritdoc/>
    public async Task<BaseResult<ChatResponseDto>> AskLlamaAsync(ChatRequestDto dto, CancellationToken ct = default)
    {
        try
        {
            using var response = await _http.PostAsJsonAsync("http://aiservice:5435/ai/chat-with-llama", dto, ct);

            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<BaseResult<ChatResponseDto>>(cancellationToken: ct));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);

            return new BaseResult<ChatResponseDto>
            {
                ErrorMessage = "Умный помощник вернул ошибку",
                ErrorCode = (int)ErrorChatCodes.AiAssistantReturnError
            };
        }
    }
}
