using Note.Domain.Interfaces.Repositories;

namespace Note.DAL.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    private readonly ApplicationDbContext _dbContext;

    public BaseRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable<TEntity> GetAll()
    {
        return _dbContext.Set<TEntity>();
    }

    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        if (entity != null)
        {
            await _dbContext.AddAsync(entity);

            return entity;
        }
        else throw new ArgumentNullException("Entity is null");
    }

    public TEntity Update(TEntity entity)
    {
        if (entity != null)
        {
            _dbContext.Update(entity);

            return entity;
        }
        else throw new ArgumentNullException("Entity is null");
    }

    public void Remove(TEntity entity)
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

