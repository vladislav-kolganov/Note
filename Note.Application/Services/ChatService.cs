using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Note.Application.Resources;
using Note.Application.Services.Extensions;
using Note.Application.Services.Helpers;
using Note.Domain.Dto.ChatDto;
using Note.Domain.Entity;
using Note.Domain.Entity.ChatEntity;
using Note.Domain.Enum;
using Note.Domain.Interfaces.Database;
using Note.Domain.Interfaces.Repositories;
using Note.Domain.Interfaces.Services;
using Note.Domain.Result;

namespace Note.Application.Services;

public class ChatService : IChatService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<Chat> _chatRepository;
    private readonly IBaseRepository<Message> _messageRepository;
    private readonly ILogger<ChatService> _logger;

    public ChatService(
        IUnitOfWork unitOfWork,
        IBaseRepository<User> userRepositoory,
        IBaseRepository<Chat> chatRepositoory,
        IBaseRepository<Message> messageRepositoory,
        ILogger<ChatService> logger)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepositoory;
        _chatRepository = chatRepositoory;
        _messageRepository = messageRepositoory;
        _logger = logger;
    }

    public async Task<BaseResult<Message>> CreateMessage(CreateMessageDto dto)
    {
        try
        {
            if (dto.textMessage == null && dto.photo.IsNullOrEmpty())
            {
                return new BaseResult<Message>()
                {
                    ErrorMessage = ErrorMessage.MessageIsEmpty,
                    ErrorCode = (int)ErrorChatCodes.MessageIsEmpty,
                };
            }

            var producerMessage = await _userRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == dto.producerMessageId);

            var consumerMessage = await _userRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == dto.consumerMessageId);


            if (producerMessage == null || consumerMessage == null)
            {
                return new BaseResult<Message>()
                {
                    ErrorMessage = ErrorMessage.UserNotFound,
                    ErrorCode = (int)ErrorCodes.UserNotFound,
                };
            }

            Chat chat;

            if (dto.chatId.HasValue && dto.chatId.Value >= 0)
            {
                chat = await _chatRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == dto.chatId.Value);
            }
            else
            {
                chat = await _chatRepository.GetAll().
                FirstOrDefaultAsync(x =>
                    (x.User1 == dto.producerMessageId && x.User2 == dto.consumerMessageId) ||
                    (x.User1 == dto.consumerMessageId && x.User2 == dto.producerMessageId));
            }
            if (chat == null)
            {
                chat = new Chat()
                {
                    User1 = dto.producerMessageId,
                    User2 = dto.consumerMessageId,
                    CreatedAt = DateTime.UtcNow,
                };

                await _chatRepository.CreateAsync(chat);
                await _unitOfWork.SaveChangeAsync();
            }

            var message = new Message()
            {
                ChatId = chat.Id,
                TextMessage = dto.textMessage,
                ProducerMessageId = dto.producerMessageId,
                ConsumerMessageId = dto.consumerMessageId,
                CreatedAt = DateTime.UtcNow
            };
            if (dto.photo.IsNotNullOrEmpty())
            {
                message.Photos = dto.photo.Where(p => p.IsNotNullOrEmpty())
                    .Select(p => new MessagePhoto { Content = p })
                    .ToList();
            }

            await _messageRepository.CreateAsync(message);
            await _unitOfWork.SaveChangeAsync();

            return new BaseResult<Message>()
            {
                Data = message
            };
        }
        catch (Exception ex)
        {
            return LogErrorHelper<Message>.LogException(ex.Message, _logger);
        }
    }

    /// <summary>
    /// Удаление чата пользователя
    /// </summary>
    /// <param name="chatId">Id чата </param>
    /// <param name="userId">Id пользователя</param>
    public async Task<BaseResult<bool>> DeleteChat(long chatId)
    {
        try
        {
            var chat = await _chatRepository.GetAll()
                 .FirstOrDefaultAsync(x => x.Id == chatId);

            if (chat == null)
            {
                return new BaseResult<bool>()
                {
                    ErrorMessage = ErrorMessage.ChatNotFound,
                    ErrorCode = (int)ErrorChatCodes.ChatNotFound
                };
            }

            _chatRepository.Remove(chat);
            await _unitOfWork.SaveChangeAsync();

            return new BaseResult<bool>
            {
                Data = true
            };
        }
        catch (Exception ex)
        {
            return LogErrorHelper<bool>.LogException(ex.Message, _logger);
        }
    }

    public async Task<BaseResult<Message>> EditMessage(EditMessageDto dto)
    {
        try
        {
            var message = await _messageRepository.GetAll().
                 FirstOrDefaultAsync(x => x.Id == dto.messageId);

            if (message == null)
            {
                return new BaseResult<Message>
                {
                    ErrorMessage = ErrorMessage.MessageNotFound,
                    ErrorCode = (int)ErrorChatCodes.MessageNotFound
                };
            }

            if (string.IsNullOrWhiteSpace(dto.textMessage))
            {
                return new BaseResult<Message>
                {
                    ErrorMessage = ErrorMessage.MessageIsEmpty,
                    ErrorCode = (int)ErrorChatCodes.MessageIsEmpty
                };
            }

            message.TextMessage = dto.textMessage;
            message.UpdatedAt = DateTime.UtcNow;

            _messageRepository.Update(message);
            await _unitOfWork.SaveChangeAsync();

            return new BaseResult<Message> { Data = message };
        }
        catch (Exception ex)
        {
            return LogErrorHelper<Message>.LogException(ex.Message, _logger);
        }
    }

    public async Task<CollectionResult<Chat>> GetChats(long userId)
    {
        try
        {
            Chat[] chats = await _chatRepository.GetAll().
                Where(x => x.User1 == userId || x.User2 == userId).
                ToArrayAsync();

            if (!chats.IsNotNullOrEmpty())
            {
                return new CollectionResult<Chat>()
                {
                    ErrorMessage = ErrorMessage.ChatNotFound,
                    ErrorCode = (int)ErrorChatCodes.ChatNotFound
                };
            }

            return new CollectionResult<Chat>()
            {
                Data = chats,
                Count = chats.Length
            };
        }
        catch (Exception ex)
        {
            return LogErrorHelper<Chat>.LogExceptionForCollection(ex.Message, _logger);
        }
    }

    public async Task<BaseResult<Message>> GetLastMessage(long chatId)
    {
        try
        {
            var lastMessage = await _messageRepository.GetAll().
                            Where(x => x.ChatId == chatId).
                            OrderByDescending(x => x.CreatedAt).
                            FirstOrDefaultAsync();

            if (lastMessage == null)
            {
                return new BaseResult<Message>
                {
                    ErrorMessage = ErrorMessage.MessageNotFound,
                    ErrorCode = (int)ErrorChatCodes.MessageNotFound
                };
            }

            return new BaseResult<Message> { Data = lastMessage };
        }
        catch (Exception ex)
        {
            return LogErrorHelper<Message>.LogException(ex.Message, _logger);
        }
    }

    public async Task<CollectionResult<Message>> GetMessages(long chatId)
    {
        try
        {
            Message[] messages = await _messageRepository.GetAll().
                                    Where(x => x.ChatId == chatId).
                                    OrderBy(x => x.CreatedAt).
                                    ToArrayAsync();

            if (messages.IsNullOrEmpty())
            {
                return new CollectionResult<Message>()
                {
                    ErrorMessage = ErrorMessage.MessageNotFound,
                    ErrorCode = (int)ErrorChatCodes.MessageNotFound
                };
            }

            return new CollectionResult<Message>
            {
                Data = messages,
                Count = messages.Length
            };
        }
        catch (Exception ex)
        {
            return LogErrorHelper<Message>.LogExceptionForCollection(ex.Message, _logger);
        }
    }
}
