using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Note.DAL.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddDataAccessLayer( this IServiceCollection services, IConfiguration configuration) // подключаем зависимости для доступа к БД
        {
            var connectionstring = configuration.GetConnectionString("MSSQL"); // хранение в переменной ключа подключения к бд

            services.AddDbContext<ApplicationDbContext>( options =>
            { 
                options.UseSqlServer(connectionstring);
            });         // регистрация DbContext

        }
    }
}
