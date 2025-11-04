using Note.Domain.Interfaces.Database;

namespace Note.Domain.Interfaces.Repositories;

public interface IBaseRepository<TEntity> : IStateSaveChanges
{
    /// <summary>
    /// Извлечь сущности.
    /// </summary>
    IQueryable<TEntity> GetAll();

    /// <summary>
    /// Создать сущность.
    /// </summary>
    /// <param name="entity">Сущность.</param>
    Task<TEntity> CreateAsync(TEntity entity);

    /// <summary>
    /// Обновить сущность.
    /// </summary>
    /// <param name="entity">Сущность.</param>
    TEntity Update(TEntity entity);

    /// <summary>
    /// Удалить сущность.
    /// </summary>
    /// <param name="entity">Сущность.</param>
    void Remove(TEntity entity);
}