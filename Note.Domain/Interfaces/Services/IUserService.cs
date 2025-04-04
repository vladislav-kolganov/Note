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
        /// <summary>
        /// Метод создания пользователя
        /// </summary>
        /// <param name="model">Модель пользователя</param>
        Task<BaseResult<UserDto>> Create(RegisterUserDto model);

        /// <summary>
        /// Метод обновления пользователя
        /// </summary>
        /// <param name="model">Модель пользователя</param>
        /// <returns></returns>
        Task<BaseResult<UserDto>> Update(UserDto model);

        /// <summary>
        /// Метод удаления пользователя
        /// </summary>
        /// <param name="id">Id пользователя</param>
        /// <returns></returns>
        Task<BaseResult<UserDto>> Delete(long id);
    }
}
