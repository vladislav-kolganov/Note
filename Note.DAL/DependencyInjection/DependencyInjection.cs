using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Note.DAL.Repositories;
using Note.Domain.Entity;
using Note.Domain.Entity.ChatEntity;
using Note.Domain.Entity.Map;
using Note.Domain.Interfaces.Database;
using Note.Domain.Interfaces.Repositories;
using Note.Domain.Settings;

namespace Note.DAL.DependencyInjection;

public static class DependencyInjection
{
    public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSettings(configuration);

        services.AddDbContext<ApplicationDbContext>();

        services.InitRepositories();
    }

    private static void InitRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IBaseRepository<User>, BaseRepository<User>>();
        services.AddScoped<IBaseRepository<Role>, BaseRepository<Role>>();
        services.AddScoped<IBaseRepository<UserRole>, BaseRepository<UserRole>>();
        services.AddScoped<IBaseRepository<UserToken>, BaseRepository<UserToken>>();
        services.AddScoped<IBaseRepository<Report>, BaseRepository<Report>>();
        services.AddScoped<IBaseRepository<Chat>, BaseRepository<Chat>>();
        services.AddScoped<IBaseRepository<Message>, BaseRepository<Message>>();
        services.AddScoped<IBaseRepository<ReportPhoto>, BaseRepository<ReportPhoto>>();
        services.AddScoped<IBaseRepository<UserReport>, BaseRepository<UserReport>>();
        services.AddScoped<IBaseRepository<ReportMapMarker>, BaseRepository<ReportMapMarker>>();
        services.AddScoped<IBaseRepository<ReportMapMarkerAttachment>, BaseRepository<ReportMapMarkerAttachment>>();
    }

    private static IServiceCollection AddSettings(this IServiceCollection services,
    IConfiguration configuration) =>
    services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)))
            .Configure<EncryptionSettings>(configuration.GetSection(nameof(EncryptionSettings)));
}