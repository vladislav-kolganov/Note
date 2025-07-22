using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Note.DAL.Interceptors;
using Note.DAL.Repositories;
using Note.Domain.Entity;
using Note.Domain.Entity.ChatEntity;
using Note.Domain.Interfaces.Database;
using Note.Domain.Interfaces.External;
using Note.Domain.Interfaces.Repositories;
using Note.Domain.Settings;
using Refit;
using System.Text.Json;

namespace Note.DAL.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSettings(configuration);

            services.AddDbContext<ApplicationDbContext>();

            //services.AddSerilog(configuration);

            services.AddRefit(configuration);

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
            services.Configure<JwtSettings>(settings => configuration.GetSection(nameof(JwtSettings)));

            return services;
        }

        public static IServiceCollection AddRefit(this IServiceCollection services,
        IConfiguration configuration)
        {
            var config = configuration.GetSection(nameof(ApiPythonSettings)).Get<ApiPythonSettings>() ??
                throw new NullReferenceException(nameof(ApiPythonSettings) + " is null");

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };

            var refitSettings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(jsonOptions)
            };

            services.AddRefitClient<IFirePrediction>(refitSettings).
            ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri($"{config.UrlApi}");
            });

            return services;
        }
    }
}
