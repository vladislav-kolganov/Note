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
    Task<CollectionResult<Chat>> GetChatsAsync(long userId);

    /// <summary>
    /// Получить сообщения в чате.
    /// </summary>
    /// <param name="chatId">Id чата.</param>
    /// <returns>Список сообщений.</returns>
    Task<CollectionResult<Message>> GetMessagesAsync(long chatId);

    /// <summary>
    /// Последнее сообщение чата, для отображения в списке чатов (отправлять будем не более 50 символов).
    /// </summary>
    /// <param name="chatId">Id чата.</param>
    /// <returns>Сокращенный вариант сообщения.</returns>
    Task<BaseResult<Message>> GetLastMessageAsync(long chatId);

    /// <summary>
    /// Удаление чата.
    /// </summary>
    /// <param name="chatId">Id чата.</param>
    Task<BaseResult<bool>> DeleteChatAsync(long chatId);

    /// <summary>
    /// Редактирование сообщения.
    /// </summary>
    /// <param name="messageId">Id сообщения.</param>
    /// <param name="textMessage">Отредактированный текст сообщения.</param>
    /// <returns>Отредактированное сообщение.</returns>
    Task<BaseResult<Message>> EditMessageAsync(EditMessageDto dto);

    /// <summary>
    /// Создать сообщение.
    /// </summary>
    /// <param name="chatId">Id чата.</param>
    /// <param name="producerMessageId">Id отправителя сообщения.</param>
    /// <param name="consumerMessageId">Id получателя сообщения.</param>
    /// <param name="textMessage">Текст сообщения.</param>
    /// <param name="photo">Фото сообщения.</param>
    /// <returns>Созданное сообщение.</returns>
    Task<BaseResult<Message>> CreateMessageAsync(CreateMessageDto dto);

    /// <summary>
    /// Создание чата.
    /// </summary>
    /// <param name="dto">Дто для создания чата.</param>
    /// <returns>Созданный чат.Ы</returns>
    Task<BaseResult<Chat>> FindOrCreateChatAsync(UserCreateChatDto dto);
}