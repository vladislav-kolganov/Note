using Note.Domain.Dto.ChatDto;
using Note.Domain.Entity.ChatEntity;
using Note.Domain.Result;

namespace Note.Domain.Interfaces.Services;

/// <summary>
/// Интерфейс сервиса чата.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Получить список чатов.
    /// </summary>
    /// <param name="userId">Id пользователя, который запрашивает список.</param>
    /// <returns>Модель чата.</returns>
    Task<CollectionResult<Chat>> GetChats(long userId);

    /// <summary>
    /// Получить сообщения в чате.
    /// </summary>
    /// <param name="chatId">Id чата.</param>
    /// <returns>Список сообщений.</returns>
    Task<CollectionResult<Message>> GetMessages(long chatId);

    /// <summary>
    /// Последнее сообщение чата, для отображения в списке чатов (отправлять будем не более 50 символов).
    /// </summary>
    /// <param name="chatId">Id чата.</param>
    /// <returns>Сокращенный вариант сообщения.</returns>
    Task<BaseResult<Message>> GetLastMessage(long chatId);

    /// <summary>
    /// Удаление чата.
    /// </summary>
    /// <param name="chatId">Id чата.</param>
    Task<BaseResult<bool>> DeleteChat(long chatId);

    /// <summary>
    /// Редактирование сообщения.
    /// </summary>
    /// <param name="messageId">Id сообщения.</param>
    /// <param name="textMessage">Отредактированный текст сообщения.</param>
    /// <returns>Отредактированное сообщение.</returns>
    Task<BaseResult<Message>> EditMessage(EditMessageDto dto);

    /// <summary>
    /// Создать сообщение.
    /// </summary>
    /// <param name="chatId">Id чата.</param>
    /// <param name="producerMessageId">Id отправителя сообщения.</param>
    /// <param name="consumerMessageId">Id получателя сообщения.</param>
    /// <param name="textMessage">Текст сообщения.</param>
    /// <param name="photo">Фото сообщения.</param>
    /// <returns>Созданное сообщение.</returns>
    Task<BaseResult<Message>> CreateMessage(CreateMessageDto dto);
}
