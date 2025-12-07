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

/// <summary>
/// Сервис чата.
/// </summary>
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

    public async Task<CollectionResult<ChatListItemDto>> GetChatListAsync(long userId)
    {
        try
        {
            var chats = await _chatRepository.GetAll().Where(x => x.User1 == userId || x.User2 == userId).
                                              ToListAsync();
            if (chats.IsNullOrEmpty())
            {
                return new CollectionResult<ChatListItemDto>()
                {
                    ErrorMessage = ErrorMessage.ChatNotFound,
                    ErrorCode = (int)ErrorChatCodes.ChatNotFound
                };
            }

            var chatIds = chats.Select(c => c.Id).ToArray();
            var partnerIds = chats.Select(c => c.User1 == userId ? c.User2 : c.User1).
                                   Distinct().
                                   ToArray();
            var partners = await _userRepository.GetAll().Where(u => partnerIds.Contains(u.Id)).
                                                 ToListAsync();

            var partnerDict = partners.ToDictionary(u => u.Id, u => u.Login);

            var messages = await _messageRepository.GetAll().Where(m => chatIds.Contains(m.ChatId)).
                                                    OrderBy(m => m.ChatId).ThenByDescending(m => m.CreatedAt).
                                                    ToListAsync();

            var lastMessagesDict = messages.GroupBy(m => m.ChatId).ToDictionary(g => g.Key, g => g.First());

            var items = chats.Select(chat =>
            {
                var partnerId = chat.User1 == userId ? chat.User2 : chat.User1;

                partnerDict.TryGetValue(partnerId, out var login);
                lastMessagesDict.TryGetValue(chat.Id, out var lastMessage);

                return new ChatListItemDto(
                    ChatId: chat.Id,
                    PartnerId: partnerId,
                    PartnerLogin: login ?? $"User {partnerId}",
                    LastMessageText: lastMessage?.TextMessage,
                    LastMessageCreatedAt: lastMessage?.CreatedAt
                );
            }).ToList();

            return new CollectionResult<ChatListItemDto>
            {
                Data = items,
                Count = items.Count
            };

        }
        catch (Exception ex)
        {
            return LogErrorHelper<ChatListItemDto>.LogExceptionForCollection(ex.Message, _logger);
        }
    }

    public async Task<BaseResult<Chat>> FindOrCreateChatAsync(UserCreateChatDto dto)
    {
        try
        {
            if (dto == null)
            {
                return new BaseResult<Chat>()
                {
                    ErrorMessage = ErrorMessage.InvalidClientRequest,
                    ErrorCode = (int)ErrorCodes.InvalidClientRequest
                };
            }

            Chat? chat;

            if (dto.IdChat.HasValue && dto.IdChat.Value >= 0)
            {
                chat = await _chatRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == dto.IdChat.Value);
            }
            else
            {
                chat = _chatRepository.GetAll().
                FirstOrDefault(x =>
                    (x.User1 == dto.IdUser1 && x.User2 == dto.IdUser2)
                    || (x.User1 == dto.IdUser2 && x.User2 == dto.IdUser1)
                );
            }
            if (chat is null)
            {
                chat = new Chat()
                {
                    User1 = dto.IdUser1,
                    User2 = dto.IdUser2,
                    CreatedAt = DateTime.UtcNow,
                };

                await _chatRepository.CreateAsync(chat);
                await _unitOfWork.SaveChangeAsync();
            }

            return new BaseResult<Chat>()
            {
                Data = chat,
            };
        }
        catch (Exception ex)
        {
            return LogErrorHelper<Chat>.LogException(ex.Message, _logger);
        }
    }

    public async Task<BaseResult<Message>> CreateMessageAsync(CreateMessageDto dto)
    {
        try
        {
            if (dto.TextMessage == null && dto.Photo.IsNullOrEmpty())
            {
                return new BaseResult<Message>()
                {
                    ErrorMessage = ErrorMessage.MessageIsEmpty,
                    ErrorCode = (int)ErrorChatCodes.MessageIsEmpty,
                };
            }

            var producerMessage = await _userRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == dto.ProducerMessageId);

            var consumerMessage = await _userRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == dto.ConsumerMessageId);


            if (producerMessage == null || consumerMessage == null)
            {
                return new BaseResult<Message>()
                {
                    ErrorMessage = ErrorMessage.UserNotFound,
                    ErrorCode = (int)ErrorCodes.UserNotFound,
                };
            }

            var chat = await FindOrCreateChatAsync(new UserCreateChatDto(dto.ProducerMessageId, dto.ConsumerMessageId, dto.ChatId));
            if (!chat.IsSuccess)
            {
                if (producerMessage == null || consumerMessage == null)
                {
                    return new BaseResult<Message>()
                    {
                        ErrorMessage = ErrorMessage.CouldntCreateAchat,
                        ErrorCode = (int)ErrorChatCodes.CouldntCreateAchat,
                    };
                }
            }

            var message = new Message()
            {
                ChatId = chat.Data.Id,
                TextMessage = dto.TextMessage,
                ProducerMessageId = dto.ProducerMessageId,
                ConsumerMessageId = dto.ConsumerMessageId,
                CreatedAt = DateTime.UtcNow
            };
            if (dto.Photo.IsNotNullOrEmpty())
            {
                message.Photos = dto.Photo.Where(p => p.IsNotNullOrEmpty())
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
    public async Task<BaseResult<bool>> DeleteChatAsync(long chatId)
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

    public async Task<BaseResult<Message>> EditMessageAsync(EditMessageDto dto)
    {
        try
        {
            var message = await _messageRepository.GetAll().
                 FirstOrDefaultAsync(x => x.Id == dto.MessageId);

            if (message is null)
            {
                return new BaseResult<Message>
                {
                    ErrorMessage = ErrorMessage.MessageNotFound,
                    ErrorCode = (int)ErrorChatCodes.MessageNotFound
                };
            }

            if (string.IsNullOrWhiteSpace(dto.TextMessage))
            {
                return new BaseResult<Message>
                {
                    ErrorMessage = ErrorMessage.MessageIsEmpty,
                    ErrorCode = (int)ErrorChatCodes.MessageIsEmpty
                };
            }

            message.TextMessage = dto.TextMessage;
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

    public async Task<CollectionResult<Chat>> GetChatsAsync(long userId)
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

    public async Task<BaseResult<Message>> GetLastMessageAsync(long chatId)
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

    public async Task<CollectionResult<Message>> GetMessagesAsync(long chatId)
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

    public async Task<BaseResult<bool>> DeleteMessagesAsync(long userId, params long[] messageIds)
    {
        if (messageIds.IsNullOrEmpty())
        {
            return new BaseResult<bool>
            {
                ErrorMessage = ErrorMessage.MessagesIdsIsEmpty,
                ErrorCode = (int)ErrorChatCodes.MessagesIdsIsEmpty
            };
        }
      
        try
        {
            var rowsAffected = await _messageRepository.GetAll()
                .Where(x => messageIds.Contains(x.Id) && x.ProducerMessageId == userId)
                .ExecuteDeleteAsync();

            if (rowsAffected == 0)
            {
                return new BaseResult<bool>
                {
                    ErrorMessage = ErrorMessage.MessageNotFound,
                    ErrorCode = (int)ErrorChatCodes.MessageNotFound
                };
            }

            return new BaseResult<bool>
            {
                Data = true
            };
        }
        catch (Exception ex)
        {
            return new BaseResult<bool>
            {
                ErrorMessage = ErrorMessage.InternalServerError,
                ErrorCode = (int)ErrorCodes.InternalServerError,
            };
        }
    }
}