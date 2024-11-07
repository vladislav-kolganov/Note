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
    public class DateInterceptor(IHttpContextAccessor httpContextAccessor) : SaveChangesInterceptor

    {
        //public DateInterceptor()
        //{

        //}
        public override  ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
         CancellationToken cancellationToken = new CancellationToken())
        {
            var dbContext = eventData.Context;
            if (dbContext == null)
            {
                return   base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            var entries = dbContext.ChangeTracker.Entries<IAuditable>()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified)
                .ToList();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property(x => x.CreatedAt).CurrentValue = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property(x => x.UpdatedAt).CurrentValue = DateTime.UtcNow;
                }
            }

            return   base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
