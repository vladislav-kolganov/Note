using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Note.DAL.Interceptors;
using Note.DAL.Repositories;
using Note.Domain.Entity;
using Note.Domain.Entity.ChatEntity;
using Note.Domain.Interfaces.Database;
using Note.Domain.Interfaces.Repositories;
using Note.Domain.Settings;
using Note.Domain.Settings.DbSettings;

namespace Note.DAL.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSettings(configuration);

            services.AddDbContext();

            services.AddSingleton<DateInterceptor>();

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
        }

        private static IServiceCollection AddSettings(this IServiceCollection services,
        IConfiguration configuration)
        {
            services.Configure<PostgresSettings>(configuration.GetSection(nameof(PostgresSettings)));
            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));

            return services;
        }
        public static IServiceCollection AddDbContext(this IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>();

            return services;
        }
    }
}
