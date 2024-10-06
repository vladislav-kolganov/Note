using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Note.DAL.Interceptors;
using Note.DAL.Repositories;
using Note.Domain.Entity;
using Note.Domain.Interfaces.Repositories;

namespace Note.DAL.DependencyInjection
{
    public static class DependencyInjection
    {

        public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration) // подключаем зависимости для доступа к БД
        {
            var connectionstring = configuration.GetConnectionString("PostgresSQL"); // хранение в переменной ключа подключения к бд

            services.AddSingleton<DateInterceptor>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionstring);
            });         // регистрация DbContext Все необходимые зависимости мы настраиваем в этом методе, который мы будем вызывать в API
            services.InitRepositories();
        }

        private static void InitRepositories(this IServiceCollection services) // регистрация репозиториев
        {

            services.AddScoped<IBaseRepository<User>, BaseRepository<User>>();
            services.AddScoped<IBaseRepository<Report>, BaseRepository<Report>>();
        }
    }
}
