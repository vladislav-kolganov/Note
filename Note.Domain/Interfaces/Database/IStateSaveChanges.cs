using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Domain.Interfaces.Database
{
    public interface IStateSaveChanges
    {
        public Task<int> SaveChangeAsync();
    }
}
