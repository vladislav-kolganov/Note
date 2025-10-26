using Note.Domain.Entity.ChatEntity;
using Note.Domain.Result;

namespace Note.Domain.Interfaces.Services;

public interface IChatService
{
    /// <summary>
    /// Получить список чатов
    /// </summary>
    /// <param name="userId">id пользователя, который запрашивает список</param>
    /// <returns>Модель чата</returns>
    Task<CollectionResult<Chat>> GetChats(long userId);

    /// <summary>
    /// Получить сообщения в чате
    /// </summary>
    /// <param name="chatId">id чата</param>
    /// <returns>Список сообщений</returns>
    Task<CollectionResult<Message>> GetMessages(long chatId);

    /// <summary>
    /// Последнее сообщение чата, для отображения в списке чатов (отправлять будем не более 50 символов)
    /// </summary>
    /// <param name="chatId">id чата</param>
    /// <returns>Сокращенный вариант сообщения</returns>
    Task<BaseResult<Message>> GetLastMessage(long chatId);

    /// <summary>
    /// Удаление чата
    /// </summary>
    /// <param name="chatId">id чата</param>
    Task<BaseResult<bool>> DeleteChat(long chatId);

    /// <summary>
    /// Редактирование сообщения
    /// </summary>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="textMessage">Отредактированный текст сообщения</param>
    /// <returns></returns>
    Task<BaseResult<Message>> EditMessage(long messageId, string textMessage);

    /// <summary>
    /// Создание сообщения
    /// </summary>
    /// <param name="chatId">id чата (если есть)</param>
    /// <param name="from">id отправителя</param>
    /// <param name="to">id получателя</param>
    /// <param name="text">текст сообщения</param>
    /// <returns></returns>
    Task<BaseResult<Message>> CreateMessage(long chatId, long producerMessageId, long consumerMessageId, string? textMessage, List<byte[]>? photo);
}
