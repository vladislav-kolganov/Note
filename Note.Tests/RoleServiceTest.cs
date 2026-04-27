using AutoMapper;
using MockQueryable.Moq;
using Moq;
using Note.Application.Mapping;
using Note.Application.Services;
using Note.Domain.Dto.RoleDto;
using Note.Domain.Entity;
using Note.Domain.Enum;
using Note.Domain.Interfaces.Repositories;
using Xunit;

namespace Note.Tests;

public class RoleServiceTest
{
    private readonly IMapper _mapper = new MapperConfiguration(cfg =>
    {
        cfg.AddProfile(new ReportMapping());
        cfg.AddProfile(new UserMapping());
        cfg.AddProfile(new RoleMapping());
    }).CreateMapper();

    private RoleServices CreateRoleService(
        Mock<IBaseRepository<User>>? userRepo = null,
        Mock<IBaseRepository<Role>>? roleRepo = null)
    {
        return new RoleServices(
            userRepo?.Object ?? GetMockUserRepository().Object,
            roleRepo?.Object ?? GetMockRoleRepository().Object,
            _mapper
        );
    }

    #region Mock Data

    private static List<Role> GetRoles()
    {
        return new List<Role>
        {
            new() { Id = 1, Name = "User" },
            new() { Id = 2, Name = "Moderator" }
        };
    }

    private static Mock<IBaseRepository<Role>> GetMockRoleRepository()
    {
        var roles = GetRoles().AsQueryable().BuildMockDbSet();
        var mock = new Mock<IBaseRepository<Role>>();
        mock.Setup(r => r.GetAll()).Returns(() => roles.Object);
        mock.Setup(r => r.CreateAsync(It.IsAny<Role>())).ReturnsAsync((Role r) => { r.Id = 10; return r; });
        mock.Setup(r => r.Update(It.IsAny<Role>())).Returns((Role r) => r);
        mock.Setup(r => r.Remove(It.IsAny<Role>()));
        mock.Setup(r => r.SaveChangeAsync()).ReturnsAsync(1);
        return mock;
    }

    private static List<User> GetUsers()
    {
        return new List<User>
        {
            new()
            {
                Id = 1, Login = "TestUser1",
                Role = new List<Role> { new() { Id = 1, Name = "User" } }
            },
            new()
            {
                Id = 2, Login = "TestUser2",
                Role = new List<Role>
                {
                    new() { Id = 1, Name = "User" },
                    new() { Id = 2, Name = "Moderator" }
                }
            }
        };
    }

    private static Mock<IBaseRepository<User>> GetMockUserRepository()
    {
        var users = GetUsers().AsQueryable().BuildMockDbSet();
        var mock = new Mock<IBaseRepository<User>>();
        mock.Setup(r => r.GetAll()).Returns(() => users.Object);
        mock.Setup(r => r.SaveChangeAsync()).ReturnsAsync(1);
        return mock;
    }

    #endregion

    #region CreateRoleAsync

    [Fact]
    public async Task CreateRoleAsync_ShouldCreateRole_WhenNameIsUnique()
    {
        // Arrange
        var service = CreateRoleService();
        var dto = new CreateRoleDto { Name = "Admin" };

        // Act
        var result = await service.CreateRoleAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("Admin", result.Data.Name);
    }

    [Fact]
    public async Task CreateRoleAsync_ShouldReturnError_WhenRoleAlreadyExists()
    {
        // Arrange
        var service = CreateRoleService();
        var dto = new CreateRoleDto { Name = "User" };

        // Act
        var result = await service.CreateRoleAsync(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.RoleAlreadyExists, result.ErrorCode);
    }

    #endregion

    #region DeleteRoleAsync

    [Fact]
    public async Task DeleteRoleAsync_ShouldDeleteRole_WhenRoleExists()
    {
        // Arrange
        var service = CreateRoleService();

        // Act
        var result = await service.DeleteRoleAsync(1);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.Id);
        Assert.Equal("User", result.Data.Name);
    }

    [Fact]
    public async Task DeleteRoleAsync_ShouldReturnError_WhenRoleNotFound()
    {
        // Arrange
        var service = CreateRoleService();

        // Act
        var result = await service.DeleteRoleAsync(999);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.RoleNotFound, result.ErrorCode);
    }

    #endregion

    #region UpdateRoleAsync

    [Fact]
    public async Task UpdateRoleAsync_ShouldReturnRole_WhenRoleExists()
    {
        // Arrange
        var service = CreateRoleService();
        var dto = new RoleDto { Id = 1, Name = "User" };

        // Act
        var result = await service.UpdateRoleAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.Id);
    }

    [Fact]
    public async Task UpdateRoleAsync_ShouldReturnError_WhenRoleNotFound()
    {
        // Arrange
        var service = CreateRoleService();
        var dto = new RoleDto { Id = 999, Name = "Ghost" };

        // Act
        var result = await service.UpdateRoleAsync(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.RoleNotFound, result.ErrorCode);
    }

    #endregion

    #region AddRoleForUserAsync

    [Fact]
    public async Task AddRoleForUserAsync_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        var service = CreateRoleService();
        var dto = new UserRoleDto { Login = "NonExistent", RoleName = "Moderator" };

        // Act
        var result = await service.AddRoleForUserAsync(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.UserNotFound, result.ErrorCode);
    }

    [Fact]
    public async Task AddRoleForUserAsync_ShouldReturnError_WhenUserAlreadyHasOnlyThatRole()
    {
        // Arrange — User 1 has only "User" role
        var service = CreateRoleService();
        var dto = new UserRoleDto { Login = "TestUser1", RoleName = "User" };

        // Act
        var result = await service.AddRoleForUserAsync(dto);

        // Assert — roles.Any(x => x != "User") is false → "already has role" error
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.UserAlreadyExistsThisRole, result.ErrorCode);
    }

    [Fact]
    public async Task AddRoleForUserAsync_ShouldThrow_WhenTryingToAddRole()
    {
        // Arrange — _userRoleRepository is never injected, always null
        var service = CreateRoleService();
        var dto = new UserRoleDto { Login = "TestUser1", RoleName = "Moderator" };

        // Act & Assert — enters add branch, then NRE on _userRoleRepository.CreateAsync
        await Assert.ThrowsAsync<NullReferenceException>(
            () => service.AddRoleForUserAsync(dto));
    }

    [Fact]
    public async Task AddRoleForUserAsync_ShouldReturnError_WhenRoleNotFoundInRepo()
    {
        // Arrange — role "NonExistent" doesn't exist in role repo
        var service = CreateRoleService();
        var dto = new UserRoleDto { Login = "TestUser1", RoleName = "NonExistent" };

        // Act
        var result = await service.AddRoleForUserAsync(dto);

        // Assert — role not found returns UserNotFound error code
        Assert.False(result.IsSuccess);
        Assert.Equal((int)ErrorCodes.UserNotFound, result.ErrorCode);
    }

    #endregion
}