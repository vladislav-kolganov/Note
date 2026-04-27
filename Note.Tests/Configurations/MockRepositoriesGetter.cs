using MockQueryable.Moq;
using Moq;
using Note.Domain.Entity;
using Note.Domain.Entity.ChatEntity;
using Note.Domain.Entity.Map;
using Note.Domain.Interfaces.Repositories;

namespace Note.Tests.Configurations;

public static class MockRepositoriesGetter
{
    #region Report Data

    public static IQueryable<Report> GetReports()
    {
        return new List<Report>
        {
            new()
            {
                Id = 1,
                Name = "Отчёт о пожаре #1",
                Description = "Описание первого отчёта",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-2),
                Photos = new List<ReportPhoto>(),
                MapMarkers = new List<ReportMapMarker>()
            },
            new()
            {
                Id = 2,
                Name = "Отчёт о пожаре #2",
                Description = "Описание второго отчёта",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                Photos = new List<ReportPhoto>(),
                MapMarkers = new List<ReportMapMarker>()
            }
        }.AsQueryable();
    }

    public static Mock<IBaseRepository<Report>> GetMockReportRepository()
    {
        var mock = new Mock<IBaseRepository<Report>>();
        var reports = GetReports().BuildMockDbSet();
        mock.Setup(r => r.GetAll()).Returns(() => reports.Object);
        mock.Setup(r => r.CreateAsync(It.IsAny<Report>())).ReturnsAsync((Report entity) => entity);
        mock.Setup(r => r.Update(It.IsAny<Report>())).Returns((Report entity) => entity);
        mock.Setup(r => r.SaveChangeAsync()).ReturnsAsync(1);
        return mock;
    }

    #endregion

    #region User Data

    public static IQueryable<User> GetUsers()
    {
        return new List<User>
        {
            new()
            {
                Id = 1,
                Login = "TestUser1",
                Password = "Hash1",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
            },
            new()
            {
                Id = 2,
                Login = "TestUser2",
                Password = "Hash2",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
            }
        }.AsQueryable();
    }

    public static Mock<IBaseRepository<User>> GetMockUserRepository()
    {
        var mock = new Mock<IBaseRepository<User>>();
        var users = GetUsers().BuildMockDbSet();
        mock.Setup(r => r.GetAll()).Returns(() => users.Object);
        return mock;
    }

    #endregion

    #region UserReport Data

    public static IQueryable<UserReport> GetUserReports()
    {
        return new List<UserReport>
        {
            new()
            {
                UserId = 1, OwnerId = 1, ReportId = 1, IsDeleteForThisUser = false
            },
            new()
            {
                UserId = 1, OwnerId = 1, ReportId = 2, IsDeleteForThisUser = false
            },
            new()
            {
                UserId = 2, OwnerId = 1, ReportId = 2, IsDeleteForThisUser = false
            }
        }.AsQueryable();
    }

    public static Mock<IBaseRepository<UserReport>> GetMockUserReportRepository()
    {
        var mock = new Mock<IBaseRepository<UserReport>>();
        var data = GetUserReports().BuildMockDbSet();
        mock.Setup(r => r.GetAll()).Returns(() => data.Object);
        mock.Setup(r => r.CreateAsync(It.IsAny<UserReport>())).ReturnsAsync((UserReport e) => e);
        mock.Setup(r => r.Update(It.IsAny<UserReport>())).Returns((UserReport e) => e);
        mock.Setup(r => r.SaveChangeAsync()).ReturnsAsync(1);
        return mock;
    }

    #endregion

    #region ReportPhoto Data

    public static Mock<IBaseRepository<ReportPhoto>> GetMockReportPhotoRepository()
    {
        var mock = new Mock<IBaseRepository<ReportPhoto>>();
        var data = new List<ReportPhoto>().AsQueryable().BuildMockDbSet();
        mock.Setup(r => r.GetAll()).Returns(() => data.Object);
        mock.Setup(r => r.CreateAsync(It.IsAny<ReportPhoto>())).ReturnsAsync((ReportPhoto e) => e);
        mock.Setup(r => r.SaveChangeAsync()).ReturnsAsync(1);
        return mock;
    }

    #endregion

    #region ReportMapMarker Data

    public static Mock<IBaseRepository<ReportMapMarker>> GetMockReportMapMarkerRepository()
    {
        var mock = new Mock<IBaseRepository<ReportMapMarker>>();
        var data = new List<ReportMapMarker>().AsQueryable().BuildMockDbSet();
        mock.Setup(r => r.GetAll()).Returns(() => data.Object);
        mock.Setup(r => r.CreateAsync(It.IsAny<ReportMapMarker>())).ReturnsAsync((ReportMapMarker e) => e);
        mock.Setup(r => r.SaveChangeAsync()).ReturnsAsync(1);
        return mock;
    }

    public static Mock<IBaseRepository<ReportMapMarkerAttachment>> GetMockReportMapMarkerAttachmentRepository()
    {
        var mock = new Mock<IBaseRepository<ReportMapMarkerAttachment>>();
        var data = new List<ReportMapMarkerAttachment>().AsQueryable().BuildMockDbSet();
        mock.Setup(r => r.GetAll()).Returns(() => data.Object);
        mock.Setup(r => r.CreateAsync(It.IsAny<ReportMapMarkerAttachment>())).ReturnsAsync((ReportMapMarkerAttachment e) => e);
        mock.Setup(r => r.SaveChangeAsync()).ReturnsAsync(1);
        return mock;
    }

    #endregion

    #region Chat Data

    public static IQueryable<Chat> GetChats()
    {
        return new List<Chat>
        {
            new()
            {
                Id = 1,
                User1 = 1,
                User2 = 2,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                Messages = GetMessages().ToList()
            }
        }.AsQueryable();
    }

    public static Mock<IBaseRepository<Chat>> GetMockChatRepository()
    {
        var mock = new Mock<IBaseRepository<Chat>>();
        var data = GetChats().BuildMockDbSet();
        mock.Setup(r => r.GetAll()).Returns(() => data.Object);
        mock.Setup(r => r.CreateAsync(It.IsAny<Chat>())).ReturnsAsync((Chat e) => { e.Id = 1; return e; });
        mock.Setup(r => r.Remove(It.IsAny<Chat>()));
        mock.Setup(r => r.SaveChangeAsync()).ReturnsAsync(1);
        return mock;
    }

    #endregion

    #region Message Data

    public static IQueryable<Message> GetMessages()
    {
        return new List<Message>
        {
            new()
            {
                Id = 1,
                ChatId = 1,
                ProducerMessageId = 1,
                ConsumerMessageId = 2,
                TextMessage = "Привет от User1",
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                Photos = new List<MessagePhoto>()
            },
            new()
            {
                Id = 2,
                ChatId = 1,
                ProducerMessageId = 2,
                ConsumerMessageId = 1,
                TextMessage = "Привет от User2",
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                Photos = new List<MessagePhoto>()
            }
        }.AsQueryable();
    }

    public static Mock<IBaseRepository<Message>> GetMockMessageRepository()
    {
        var mock = new Mock<IBaseRepository<Message>>();
        var data = GetMessages().BuildMockDbSet();
        mock.Setup(r => r.GetAll()).Returns(() => data.Object);
        mock.Setup(r => r.CreateAsync(It.IsAny<Message>())).ReturnsAsync((Message e) => { e.Id = 3; return e; });
        mock.Setup(r => r.Update(It.IsAny<Message>())).Returns((Message e) => e);
        mock.Setup(r => r.SaveChangeAsync()).ReturnsAsync(1);
        return mock;
    }

    #endregion
}
