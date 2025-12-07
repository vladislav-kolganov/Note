namespace Note.Domain.Enum;

/// <summary>
/// Ошибки чата.
/// </summary>
public enum ErrorChatCodes
{
    ChatNotFound = 0,
    CouldntCreateAchat =1,
    MessageNotFound = 2,
    MessageIsEmpty = 3,
    UserLoginIsEmpty = 4,
    MessagesIdsIsEmpty = 5,
    AiAssistantReturnError = 10
}