using Note.Domain.Dto.UserDto;
using Note.Domain.Entity;
using Note.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Domain.Interfaces.Services
{
    /// <summary>
    /// Интерфейс сервиса пользователя
    /// </summary>
    public interface IUserService
    {
        Task<BaseResult<UserDto>> Create(User model);

        Task<BaseResult<UserDto>> Update(User model);

        Task<BaseResult<UserDto>> Delete(long id);
    }
}
