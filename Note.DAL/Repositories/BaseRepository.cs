using Note.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.DAL.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _dbContext;

        public BaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<TEntity> GetAll() // получаем все объекты
        {
            return _dbContext.Set<TEntity>();
        }

        public Task<TEntity> CreateAsync(TEntity entity) // добавляем в бд все объекты
        {
            if (entity != null)
            {
                _dbContext.Add(entity); //  добавляем объект в контекст
                _dbContext.SaveChanges(); // фиксируем изменения в бд
                return Task.FromResult(entity);
            }
            else throw new ArgumentNullException("Entity is null"); 
        }      

        public Task<TEntity> UpdateAsync(TEntity entity) // обновляем в бд все объекты
        {
            if (entity != null)
            {
                _dbContext.Update(entity);
                _dbContext.SaveChanges();
                return Task.FromResult(entity);
            }
            else throw new ArgumentNullException("Entity is null");
        }

        public Task<TEntity> RemoveAsync(TEntity entity) // удаляем в бд все объекты
        {
            if (entity != null)
            {
                _dbContext.Remove(entity);
                _dbContext.SaveChanges();
                return Task.FromResult(entity);
            }
            else throw new ArgumentNullException("Entity is null");
        }
    }
    
}
