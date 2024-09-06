using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Domain.Result
{
    public class BaseResult // хранение информации об ответе сервиса
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}
