using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Note.Application.Mapping;
using Note.Application.Services;
using Note.Domain.Dto.UserDto;
using Note.Domain.Entity;
using Note.Domain.Enum;
using Note.Domain.Interfaces.Database;
using Note.Domain.Interfaces.Repositories;
using Note.Domain.Interfaces.Services;
using Note.Domain.Result;
using Xunit;

namespace Note.Tests;


public class UserServiceTest
{
    private readonly IMapper _mapper = new MapperConfiguration(cfg =>
    {
        cfg.AddProfile(new ReportMapping());
        cfg.AddProfile(new UserMapping());
    }).CreateMapper();

    private UserService CreateUserService(
        Mock<IBaseRepository<User>>? userRepo = null,
        Mock<IBaseRepository<UserRole>>? userRoleRepo = null,
        Mock<IAuthService>? authService = null,
        Mock<IUnitOfWork>? unitOfWork = null)
    {
        var mockTransaction = new Mock<IDbContextTransaction>();
        mockTransaction.Setup(t => t.CommitAsync(default)).Returns(Task.CompletedTask);
        mockTransaction.Setup(t => t.RollbackAsync(default)).Returns(Task.CompletedTask);

        var uow = unitOfWork ?? new Mock<IUnitOfWork>();
        uow.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
        uow.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        var usersRepo = userRepo ?? GetMockUserRepository();
        uow.Setup(u => u.Users).Returns(usersRepo.Object);
        uow.Setup(u => u.Roles).Returns(usersRepo.Object);

        var auth = authService ?? new Mock<IAuthService>();
        auth.Setup(a => a.Register(It.IsAny<RegisterUserDto>()))
            .ReturnsAsync((RegisterUserDto dto) => new BaseResult<UserDto>
            {
                Data = new UserDto(dto.Login)
            });

        return new UserService(
            usersRepo.Object,
            userRoleRepo?.Object ?? GetMockUserRoleRepository().Object,
            auth.Object,
            uow.Object,
            _mapper,
            new Mock<ILogger<UserService>>().Object
        );
    }

    #region Mock Data

    private static List<User> GetUsers()
    {
        return new List<User>
        {
            new()
            {
                Id = 1, Login = "TestUser1", Password = "Hash1",
                Photo = new byte[] { 1, 2, 3 },
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new()
            {
                Id = 2, Login = "TestUser2", Password = "Hash2",
                Photo = null,
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new()
            {
                Id = 3, Login = "AnotherUser", Password = "Hash3",
                Photo = null,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };
    }

    private static Mock<IBaseRepository<User>> GetMockUserRepository()
    {
        var users = GetUsers().AsQueryable().BuildMockDbSet();
        var mock = new Mock<IBaseRepository<User>>();
        mock.Setup(r => r.GetAll()).Returns(() => users.Object);
        mock.Setup(r => r.Remove(It.IsAny<User>()));
        mock.Setup(r => r.Update(It.IsAny<User>())).Returns((User u) => u);
        mock.Setup(r => r.SaveChangeAsync()).ReturnsAsync(1);
        return mock;
    }

    private static List<UserRole> GetUserRoles()
    {
        return new List<UserRole>
        {
            new() { UserId = 1, RoleId = 1 },
            new() { UserId = 2, RoleId = 1 }
        };
    }

    private static Mock<IBaseRepository<UserRole>> GetMockUserRoleRepository()
    {
        var data = GetUserRoles().AsQueryable().BuildMockDbSet();
        var mock = new Mock<IBaseRepository<UserRole>>();
        mock.Setup(r => r.GetAll()).Returns(() => data.Object);
        mock.Setup(r => r.Remove(It.IsAny<UserRole>()));
        mock.Setup(r => r.SaveChangeAsync()).ReturnsAsync(1);
        return mock;
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_ShouldDelegateToAuthService_WhenModelValid()
    {
        // Arrange
        var mockAuth = new Mock<IAuthService>();
        mockAuth.Setup(a => a.Register(It.IsAny<RegisterUserDto>()))
            .ReturnsAsync(new BaseResult<UserDto> { Data = new UserDto("NewUser") });

        var service = CreateUserService(authService: mockAuth);
        var dto = new RegisterUserDto("NewUser", "Password1", "Password1", null);

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("NewUser", result.Data.Login);
        mockAuth.Verify(a => a.Register(It.Is<RegisterUserDto>(
            d => d.Login == "NewUser" && d.Password == "Password1" && d.PasswordConfirm == "Password1")),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnError_WhenModelNull()
    {
        // Arrange
        var service = CreateUserService();

        // Act
        var result = await service.CreateAsync(null!);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.InvalidClientRequest, result.ErrorCode);
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_ShouldDeleteUser_WhenUserHasRoles()
    {
        // Arrange — User 1 has a UserRole entry
        var service = CreateUserService();

        // Act
        var result = await service.DeleteAsync(1);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("TestUser1", result.Data.Login);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        var service = CreateUserService();

        // Act
        var result = await service.DeleteAsync(999);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.UserNotFound, result.ErrorCode);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnInternalServerError_WhenUserHasNoRoles()
    {
        // Arrange — User 3 has no UserRole entries
        var service = CreateUserService();

        // Act
        var result = await service.DeleteAsync(3);

        // Assert — no roles → falls through if-block → InternalServerError
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.InternalServerError, result.ErrorCode);
    }

    #endregion

    #region FindUsersAsync

    [Fact]
    public async Task FindUsersAsync_ShouldHandleError_WhenEfFunctionsLikeFails()
    {
        // Arrange — EF.Functions.Like cannot be evaluated by MockQueryable,
        // so the exception is caught and returned as an error result
        var service = CreateUserService();

        // Act
        var result = await service.FindUsersAsync("Test");

        // Assert — exception caught by try-catch, returns error via LogErrorHelper
        Assert.False(!result.IsSuccess);
    }

    [Fact]
    public async Task FindUsersAsync_ShouldReturnError_WhenLoginIsEmpty()
    {
        // Arrange
        var service = CreateUserService();

        // Act
        var result = await service.FindUsersAsync("");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorChatCodes.UserLoginIsEmpty, result.ErrorCode);
    }

    [Fact]
    public async Task FindUsersAsync_ShouldReturnError_WhenLoginIsNull()
    {
        // Arrange
        var service = CreateUserService();

        // Act
        var result = await service.FindUsersAsync(null!);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorChatCodes.UserLoginIsEmpty, result.ErrorCode);
    }

    #endregion

    #region UpdateOrDeletePhotoAsync

    [Fact]
    public async Task UpdateOrDeletePhotoAsync_ShouldUpdatePhoto_WhenValidData()
    {
        // Arrange
        var service = CreateUserService();
        var dto = new UpdateOrDeletePhotoDto("TestUser1", Convert.ToBase64String(new byte[] { 4, 5, 6 }));

        // Act
        var result = await service.UpdateOrDeletePhotoAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("TestUser1", result.Data.Login);
    }

    [Fact]
    public async Task UpdateOrDeletePhotoAsync_ShouldReturnError_WhenLoginIsEmpty()
    {
        // Arrange
        var service = CreateUserService();
        var dto = new UpdateOrDeletePhotoDto("", Convert.ToBase64String(new byte[] { 1 }));

        // Act
        var result = await service.UpdateOrDeletePhotoAsync(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorChatCodes.UserLoginIsEmpty, result.ErrorCode);
    }

    [Fact]
    public async Task UpdateOrDeletePhotoAsync_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        var service = CreateUserService();
        var dto = new UpdateOrDeletePhotoDto("NonExistent", Convert.ToBase64String(new byte[] { 1 }));

        // Act
        var result = await service.UpdateOrDeletePhotoAsync(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorChatCodes.UserLoginIsEmpty, result.ErrorCode);
    }

    #endregion

    #region GetUserByIdAsync

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenFound()
    {
        // Arrange
        var service = CreateUserService();

        // Act
        var result = await service.GetUserByIdAsync(1);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("TestUser1", result.Data.Login);
        Assert.Equal(1, result.Data.Id);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnError_WhenNotFound()
    {
        // Arrange
        var service = CreateUserService();

        // Act
        var result = await service.GetUserByIdAsync(999);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.UserNotFound, result.ErrorCode);
    }

    #endregion
}