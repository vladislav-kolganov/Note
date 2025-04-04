using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Note.DAL.Interceptors;
using Note.Domain.Settings.DbSettings;
using System.Reflection;

namespace Note.DAL
{
    public class ApplicationDbContext : DbContext
    {
        private readonly PostgresSettings _postgresSettings;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IOptions<PostgresSettings> postgresSettings) : base(options)
        {
            _postgresSettings = postgresSettings.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) // DbContextOptionsBuilder optionsBuilder: Параметр, который предоставляет API для настройки DbContext.                                          // С помощью него можно добавлять различные параметры конфигурации.
        {
            optionsBuilder.UseNpgsql(_postgresSettings.ConnectionString, npgsqlOptions =>
            {
                    // Указываем сборку, в которой лежат миграции
                    npgsqlOptions.MigrationsAssembly("Note.DAL");
            });


            IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
            optionsBuilder.AddInterceptors(new DateInterceptor(httpContextAccessor));

            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);

            // Обычно здесь можно добавлять настройки для подключения к базе данных (например, строку подключения).
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }


    }
}
