using Microsoft.EntityFrameworkCore.Storage;
using Note.Domain.Entity;
using Note.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Domain.Interfaces.Database
{
    public interface IUnitOfWork :IStateSaveChanges
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
  
        IBaseRepository<User> Users { get; set; }
        IBaseRepository<User> Roles { get; set; }
        IBaseRepository<UserRole> UserRoles { get; set; }

    }
}
