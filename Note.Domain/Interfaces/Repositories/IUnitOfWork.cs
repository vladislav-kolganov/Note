using Microsoft.EntityFrameworkCore.Storage;
using Note.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<int> SaveChangeAsync();

        IBaseRepository<User> Users { get; set; }
        IBaseRepository<User> Roles { get; set; }
        IBaseRepository<UserRole> UserRoles { get; set; }

    }
}
