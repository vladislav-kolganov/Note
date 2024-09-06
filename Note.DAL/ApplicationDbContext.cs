using Microsoft.EntityFrameworkCore;
using Note.DAL.Configurations;
using Note.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Note.DAL.Interceptors;

namespace Note.DAL
{
    public class ApplicationDbContext: DbContext
    {
        ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { 
            Database.EnsureCreated(); // проверка на существование БД, если нет - то создание бд, иначе - инициализация с бд 
        }

        protected  override void OnConfiguring (DbContextOptionsBuilder optionsBuilder) // DbContextOptionsBuilder optionsBuilder: Параметр, который предоставляет API для настройки DbContext.
                                                                               // С помощью него можно добавлять различные параметры конфигурации.
        { 
            IHttpContextAccessor  httpContextAccessor = new HttpContextAccessor();
            optionsBuilder.AddInterceptors(new DateInterceptor(httpContextAccessor));

          base.OnConfiguring(optionsBuilder); // Вызывается метод базового класса, что позволяет производному классу выполнять дополнительную конфигурацию.
                                              // Обычно здесь можно добавлять настройки для подключения к базе данных (например, строку подключения).
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) //  Параметр, который предоставляет API для конфигурации модели данных, используемой в контексте.
                                                                           //  Здесь вы можете задавать конфигурации для сущностей (например, их свойства, отношения и т.д.)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); // применяем все конфигурации в сборке
          //  new UserConfiguration().Configure(modelBuilder.Entity<User>()); // применяем отдельную конфигурацию 
            base.OnModelCreating(modelBuilder); //Вызывает метод базового класса, позволяя выполнить его логику перед добавлением специфичных для вашего контекста конфигураций.
                                   //Обычно в этом методе настраиваются отношения между сущностями, ограничения и другие аспекты модели данных.
        }


    }
}
