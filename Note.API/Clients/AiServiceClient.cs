using Note.Domain.Dto.AiChatDto;

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

    public async Task<ChatResponseDto> AskLlamaAsync(ChatRequestDto dto, CancellationToken ct = default)
    {
        try
        {
            using var response = await _http.PostAsJsonAsync("http://aiservice.api:5435/ai/chat-with-llama", dto, ct);

            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<ChatResponseDto>(cancellationToken: ct));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);

            return null;
        }
    }
}
