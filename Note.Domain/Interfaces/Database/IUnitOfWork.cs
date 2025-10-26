using Microsoft.EntityFrameworkCore.Storage;
using Note.Domain.Entity;
using Note.Domain.Interfaces.Repositories;

namespace Note.Domain.Interfaces.Database;

public interface IUnitOfWork : IStateSaveChanges
{
    /// <summary>
    /// Начать транзакцию.
    /// </summary>
    Task<IDbContextTransaction> BeginTransactionAsync();

    IBaseRepository<User> Users { get; set; }
    IBaseRepository<User> Roles { get; set; }
    IBaseRepository<UserRole> UserRoles { get; set; }
}
