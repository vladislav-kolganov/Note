using Microsoft.EntityFrameworkCore.Storage;
using Note.Domain.Entity;
using Note.Domain.Interfaces.Database;
using Note.Domain.Interfaces.Repositories;

namespace Note.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context, IBaseRepository<User> users, IBaseRepository<User> roles, IBaseRepository<UserRole> userRoles)
        {
            _context = context;
            Users = users;
            Roles = roles;
            UserRoles = userRoles;
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task<int> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public IBaseRepository<User> Users { get; set; }
        public IBaseRepository<User> Roles { get; set; }
        public IBaseRepository<UserRole> UserRoles { get; set; }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
