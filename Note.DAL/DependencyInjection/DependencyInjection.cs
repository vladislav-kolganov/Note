using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Note.DAL.Interceptors;
using Note.DAL.Repositories;
using Note.Domain.Entity;
using Note.Domain.Interfaces.Database;
using Note.Domain.Interfaces.Repositories;
using Note.Domain.Settings;
using Note.Domain.Settings.DbSettings;

namespace Note.DAL.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration) // подключаем зависимости для доступа к БД
        {
            services.AddSettings(configuration);

            services.AddSingleton<DateInterceptor>();

            var postgresSettings = configuration.GetSection(nameof(PostgresSettings));

            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                var postgresSettings = serviceProvider
                    .GetRequiredService<IOptions<PostgresSettings>>()
                    .Value;

                options.UseNpgsql(postgresSettings.ConnectionString);
            });
            services.InitRepositories();
        }

        private static void InitRepositories(this IServiceCollection services) // регистрация репозиториев
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBaseRepository<User>, BaseRepository<User>>();
            services.AddScoped<IBaseRepository<Role>, BaseRepository<Role>>();
            services.AddScoped<IBaseRepository<UserRole>, BaseRepository<UserRole>>();
            services.AddScoped<IBaseRepository<UserToken>, BaseRepository<UserToken>>();
            services.AddScoped<IBaseRepository<Report>, BaseRepository<Report>>();
        }

        private static IServiceCollection AddSettings(this IServiceCollection services,
        IConfiguration configuration)
        {
            services.Configure<PostgresSettings>(options => 
            {
                configuration.GetSection(nameof(PostgresSettings)); 
            });
            
            services.Configure<JwtSettings>(options => 
            {
                configuration.GetSection(nameof(JwtSettings)); 
            });

            return services;
        }
    }
}
