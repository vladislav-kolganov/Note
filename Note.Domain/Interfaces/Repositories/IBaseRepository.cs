using Note.Domain.Interfaces.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Domain.Interfaces.Repositories
{
    public interface IBaseRepository<TEntity> : IStateSaveChanges
    {   // прописываем CRUD операции
        IQueryable<TEntity> GetAll(); // select - извлеение записей из бд

       Task <TEntity> CreateAsync (TEntity entity); // insert

        TEntity Update (TEntity entity); // update 

        void  Remove (TEntity entity);  // delete
     

    }
}
