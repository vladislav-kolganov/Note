using Microsoft.AspNetCore.SignalR;
using Note.Domain.Dto.ChatDto;
using Note.Domain.Interfaces.Services;

namespace Note.API.Hubs;

/// <summary>
/// Хаб чата.
/// </summary>
public class ChatHub : Hub
{
    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    /// <summary>
    /// Подключиться к чату: подписываем коннект на группу и сразу шлём историю.
    /// </summary>
    public async Task RegisterToChat(long chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());

        var messagesResult = await _chatService.GetMessages(chatId);

        if (messagesResult.IsSuccess)
        {
            await Clients.Caller.SendAsync("SuccessRegistration", messagesResult.Data.ToArray());
        }
        else
        {
            await Clients.Caller.SendAsync("RegistrationError",
                messagesResult.ErrorCode,
                messagesResult.ErrorMessage);
        }
    }

    /// <summary>
    /// Отключиться от чата.
    /// </summary>
    public async Task UnregisterFromChat(long chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
    }

    /// <summary>
    /// Создать сообщение и разослать в чат.
    /// </summary>
    public async Task CreateMessage(CreateMessageDto dto)
    {
        var result = await _chatService.CreateMessage(dto);

        if (!result.IsSuccess)
        {
            // скажем только отправителю, что не вышло
            await Clients.Caller.SendAsync("CreateMessageError",
                result.ErrorCode,
                result.ErrorMessage);
           
            return;
        }

        var created = result.Data!;

        var chatGroup = created.ChatId.ToString();

        await Clients.Group(chatGroup).SendAsync("NewMessage", created);
    }
}
