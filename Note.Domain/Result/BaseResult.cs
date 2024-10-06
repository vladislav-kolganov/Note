using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Domain.Result
{
    public class BaseResult // хранение информации об ответе сервиса
    {
        public bool IsSuccess => ErrorMessage == null;
        public string ErrorMessage { get; set; }
        public int? ErrorCode { get; set; } 
    }

    public class BaseResult<T> : BaseResult
    { 
        public T Data { get; set; }
    
        public BaseResult() { }
        public BaseResult(string errorMassage,int? errorCode,T data)
        {
            ErrorMessage = errorMassage;
            ErrorCode = errorCode;
            Data = data;    
        }
    }
}
