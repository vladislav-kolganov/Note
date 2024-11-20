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

        public  async Task<TEntity> CreateAsync(TEntity entity) // добавляем в бд все объекты
        {
            if (entity != null)
            {
               await _dbContext.AddAsync(entity); //  добавляем объект в контекст
        
                return entity;
            }
            else throw new ArgumentNullException("Entity is null"); 
        }      

        public  TEntity Update(TEntity entity) // обновляем в бд все объекты
        {
            if (entity != null)
            {
                _dbContext.Update(entity);
        
                return entity;
            }
            else throw new ArgumentNullException("Entity is null");
        }

        public void Remove(TEntity entity) // удаляем в бд все объекты
        {
            if (entity != null)
            {
                _dbContext.Remove(entity);
        
               
            }
            else throw new ArgumentNullException("Entity is null");
        }

        public async Task<int> SaveChangeAsync()
        {
           return await _dbContext.SaveChangesAsync();
        }
    }
    
}
