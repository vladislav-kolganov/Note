using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Note.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Note.DAL.Interceptors
{                                /* конструктор */   
    public class DateInterceptor (IHttpContextAccessor httpContextAccessor) : SaveChangesInterceptor

    {

  
        private long GetUserId()
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long userId))
                {
                    return userId;
                }
            }
            return 0;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            var dbContext = eventData.Context;
            if (dbContext == null)
            {
                return base.SavingChanges(eventData, result);
            }

            var entries = dbContext.ChangeTracker.Entries<IAuditable>(); // хранение свойств из интерфейса IAuditable

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added) // проверка на добавление объекта
                { 
                    entry.Property(x => x.CreatedAt).CurrentValue = DateTime.UtcNow; // присвоение значение полю "дата создания" 
                    entry.Property(x => x.CreatedBy).CurrentValue = GetUserId();
                }
                if (entry.State == EntityState.Modified) // проверка на изменение объекта
                { 
                    entry.Property(x=>x.UpdatedAt).CurrentValue = DateTime.UtcNow;
                    entry.Property(x => x.UpdatedBy).CurrentValue = GetUserId();
                }

              
            }

            return base.SavingChanges(eventData, result);
        }

    }
}
