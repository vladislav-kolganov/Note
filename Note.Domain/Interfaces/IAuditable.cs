using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Domain.Interfaces
{
    public interface IAuditable
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }

    }
}
