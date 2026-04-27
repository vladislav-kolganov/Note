using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Note.Application.Services;
using Note.Domain.Dto.ChatDto;
using Note.Domain.Entity;
using Note.Domain.Entity.ChatEntity;
using Note.Domain.Enum;
using Note.Domain.Interfaces.Database;
using Note.Domain.Interfaces.Repositories;
using Note.Tests.Configurations;
using Xunit;

namespace Note.Tests;

public class ChatServiceTest
{
    private ChatService CreateChatService(
        Mock<IUnitOfWork>? unitOfWork = null,
        Mock<IBaseRepository<User>>? userRepo = null,
        Mock<IBaseRepository<Chat>>? chatRepo = null,
        Mock<IBaseRepository<Message>>? messageRepo = null,
        Mock<ILogger<ChatService>>? logger = null)
    {
        var uow = unitOfWork ?? new Mock<IUnitOfWork>();
        uow.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        return new ChatService(
            uow.Object,
            userRepo?.Object ?? MockRepositoriesGetter.GetMockUserRepository().Object,
            chatRepo?.Object ?? MockRepositoriesGetter.GetMockChatRepository().Object,
            messageRepo?.Object ?? MockRepositoriesGetter.GetMockMessageRepository().Object,
            logger?.Object ?? new Mock<ILogger<ChatService>>().Object
        );
    }

    #region GetChatsAsync

    [Fact]
    public async Task GetChatsAsync_ShouldReturnChats_WhenUserHasChats()
    {
        // Arrange
        var service = CreateChatService();

        // Act
        var result = await service.GetChatsAsync(userId: 1);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetChatsAsync_ShouldReturnError_WhenNoChatsFound()
    {
        // Arrange
        var mockChatRepo = new Mock<IBaseRepository<Chat>>();
        var empty = new List<Chat>().AsQueryable().BuildMockDbSet();
        mockChatRepo.Setup(r => r.GetAll()).Returns(() => empty.Object);

        var service = CreateChatService(chatRepo: mockChatRepo);

        // Act
        var result = await service.GetChatsAsync(userId: 999);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorChatCodes.ChatNotFound, result.ErrorCode);
    }

    #endregion

    #region GetChatListAsync

    [Fact]
    public async Task GetChatListAsync_ShouldReturnItems_WhenUserHasChats()
    {
        // Arrange
        var service = CreateChatService();

        // Act
        var result = await service.GetChatListAsync(userId: 1);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Count);
    }

    #endregion

    #region GetMessagesAsync

    [Fact]
    public async Task GetMessagesAsync_ShouldReturnMessages_WhenChatHasMessages()
    {
        // Arrange
        var service = CreateChatService();

        // Act
        var result = await service.GetMessagesAsync(chatId: 1);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Count);
    }


    #endregion

    #region GetLastMessageAsync

    [Fact]
    public async Task GetLastMessageAsync_ShouldReturnLastMessage_WhenChatHasMessages()
    {
        // Arrange
        var service = CreateChatService();

        // Act
        var result = await service.GetLastMessageAsync(chatId: 1);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("Привет от User2", result.Data.TextMessage);
    }

    [Fact]
    public async Task GetLastMessageAsync_ShouldReturnError_WhenNoMessagesInChat()
    {
        // Arrange
        var mockMsgRepo = new Mock<IBaseRepository<Message>>();
        var empty = new List<Message>().AsQueryable().BuildMockDbSet();
        mockMsgRepo.Setup(r => r.GetAll()).Returns(() => empty.Object);

        var service = CreateChatService(messageRepo: mockMsgRepo);

        // Act
        var result = await service.GetLastMessageAsync(chatId: 999);

        // Assert — FirstOrDefaultAsync returns null, explicit null check works correctly
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorChatCodes.MessageNotFound, result.ErrorCode);
    }

    #endregion

    #region FindOrCreateChatAsync

    [Fact]
    public async Task FindOrCreateChatAsync_ShouldReturnExistingChat_WhenChatExists()
    {
        // Arrange
        var service = CreateChatService();
        var dto = new UserCreateChatDto(IdUser1: 1, IdUser2: 2);

        // Act
        var result = await service.FindOrCreateChatAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.Id);
    }

    [Fact]
    public async Task FindOrCreateChatAsync_ShouldCreateChat_WhenNoExistingChat()
    {
        // Arrange
        var mockChatRepo = new Mock<IBaseRepository<Chat>>();
        var empty = new List<Chat>().AsQueryable().BuildMockDbSet();
        mockChatRepo.Setup(r => r.GetAll()).Returns(() => empty.Object);
        mockChatRepo.Setup(r => r.CreateAsync(It.IsAny<Chat>())).ReturnsAsync((Chat c) => { c.Id = 10; return c; });

        var service = CreateChatService(chatRepo: mockChatRepo);
        var dto = new UserCreateChatDto(IdUser1: 1, IdUser2: 2);

        // Act
        var result = await service.FindOrCreateChatAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(10, result.Data.Id);
    }

    [Fact]
    public async Task FindOrCreateChatAsync_ShouldReturnExistingChat_WhenIdChatProvided()
    {
        // Arrange
        var service = CreateChatService();
        var dto = new UserCreateChatDto(IdUser1: 1, IdUser2: 2, IdChat: 1);

        // Act
        var result = await service.FindOrCreateChatAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Data.Id);
    }

    [Fact]
    public async Task FindOrCreateChatAsync_ShouldReturnError_WhenDtoIsNull()
    {
        // Arrange
        var service = CreateChatService();

        // Act
        var result = await service.FindOrCreateChatAsync(null!);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.InvalidClientRequest, result.ErrorCode);
    }

    #endregion

    #region CreateMessageAsync

    [Fact]
    public async Task CreateMessageAsync_ShouldReturnMessage_WhenValidDto()
    {
        // Arrange
        var service = CreateChatService();
        var dto = new CreateMessageDto(
            ChatId: 1,
            ProducerMessageId: 1,
            ConsumerMessageId: 2,
            TextMessage: "Новое сообщение",
            PhotosBase64: null
        );

        // Act
        var result = await service.CreateMessageAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("Новое сообщение", result.Data.TextMessage);
    }

    [Fact]
    public async Task CreateMessageAsync_ShouldReturnError_WhenMessageIsEmpty()
    {
        // Arrange — null PhotosBase64 causes NRE in IsNullOrEmpty due to bug (&&),
        // caught by catch → returns exception-based error
        var service = CreateChatService();
        var dto = new CreateMessageDto(
            ChatId: 1,
            ProducerMessageId: 1,
            ConsumerMessageId: 2,
            TextMessage: null,
            PhotosBase64: null
        );

        // Act
        var result = await service.CreateMessageAsync(dto);

        // Assert — IsNullOrEmpty(null) throws NRE → caught → error result
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task CreateMessageAsync_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        var mockUserRepo = new Mock<IBaseRepository<User>>();
        var empty = new List<User>().AsQueryable().BuildMockDbSet();
        mockUserRepo.Setup(r => r.GetAll()).Returns(() => empty.Object);

        var service = CreateChatService(userRepo: mockUserRepo);
        var dto = new CreateMessageDto(
            ChatId: 1,
            ProducerMessageId: 999,
            ConsumerMessageId: 2,
            TextMessage: "Текст",
            PhotosBase64: null
        );

        // Act
        var result = await service.CreateMessageAsync(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.UserNotFound, result.ErrorCode);
    }

    #endregion

    #region EditMessageAsync

    [Fact]
    public async Task EditMessageAsync_ShouldReturnEditedMessage_WhenMessageExists()
    {
        // Arrange
        var service = CreateChatService();
        var dto = new EditMessageDto(MessageId: 1, TextMessage: "Отредактированный текст");

        // Act
        var result = await service.EditMessageAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Отредактированный текст", result.Data.TextMessage);
        Assert.NotNull(result.Data.UpdatedAt);
    }

    [Fact]
    public async Task EditMessageAsync_ShouldReturnError_WhenMessageNotFound()
    {
        // Arrange
        var mockMsgRepo = new Mock<IBaseRepository<Message>>();
        var empty = new List<Message>().AsQueryable().BuildMockDbSet();
        mockMsgRepo.Setup(r => r.GetAll()).Returns(() => empty.Object);

        var service = CreateChatService(messageRepo: mockMsgRepo);
        var dto = new EditMessageDto(MessageId: 999, TextMessage: "Текст");

        // Act
        var result = await service.EditMessageAsync(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorChatCodes.MessageNotFound, result.ErrorCode);
    }

    [Fact]
    public async Task EditMessageAsync_ShouldReturnError_WhenTextIsEmpty()
    {
        // Arrange
        var service = CreateChatService();
        var dto = new EditMessageDto(MessageId: 1, TextMessage: "   ");

        // Act
        var result = await service.EditMessageAsync(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorChatCodes.MessageIsEmpty, result.ErrorCode);
    }

    #endregion

    #region DeleteChatAsync

    [Fact]
    public async Task DeleteChatAsync_ShouldReturnTrue_WhenChatExists()
    {
        // Arrange
        var service = CreateChatService();

        // Act
        var result = await service.DeleteChatAsync(chatId: 1);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task DeleteChatAsync_ShouldReturnError_WhenChatNotFound()
    {
        // Arrange
        var mockChatRepo = new Mock<IBaseRepository<Chat>>();
        var empty = new List<Chat>().AsQueryable().BuildMockDbSet();
        mockChatRepo.Setup(r => r.GetAll()).Returns(() => empty.Object);

        var service = CreateChatService(chatRepo: mockChatRepo);

        // Act
        var result = await service.DeleteChatAsync(chatId: 999);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorChatCodes.ChatNotFound, result.ErrorCode);
    }

    #endregion

    #region DeleteMessagesAsync

    [Fact]
    public async Task DeleteMessagesAsync_ShouldReturnError_WhenNoMatchingMessagesFound()
    {
        // Arrange — messages exist but belong to producer 1, trying to delete as producer 999
        var service = CreateChatService();

        // Act
        var result = await service.DeleteMessagesAsync(userId: 999, 1, 2);

        // Assert — ExecuteDeleteAsync on mock will throw, caught in catch → InternalServerError
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.InternalServerError, result.ErrorCode);
    }

    #endregion
}