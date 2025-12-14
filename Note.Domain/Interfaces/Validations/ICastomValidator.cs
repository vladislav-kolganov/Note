using Note.Domain.Entity;
using Note.Domain.Result;

namespace Note.Domain.Interfaces.Validations;

public interface ICastomValidator : IBaseValidator<Report>
{
    /// <summary>
    /// Проверяется пользователь, если пользователя с userId не найден в БД, то такого пользователя нет.
    /// </summary>
    BaseResult UserExistValidator(User user);
}