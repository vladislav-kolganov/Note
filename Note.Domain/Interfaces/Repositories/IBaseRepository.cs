using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Domain.Interfaces.Repositories
{
    public interface IBaseRepository<TEntity> 
    {   // прописываем CRUD операции
        IQueryable<TEntity> GetAll(); // select - извлеение записей из бд

        Task<TEntity> CreateAsync (TEntity entity); // insert

        Task<TEntity> UpdateAsync (TEntity entity); // update 

        Task<TEntity>  RemoveAsync (TEntity entity);  // delete

    }
}
