using Note.Domain.Entity;
using Note.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Domain.Interfaces.Validations
{
    public interface IReportValidator: IBaseValidator<Report>
    {
    /// <summary>
    /// Проверяется наличие отчёта, если отчёт с переданным названием уже есть в БД, то создать с таким же именем нельзя
    /// Проверяется пользователь, если пользователя с userId не найден в БД, то такого пользователя нет
    /// </summary>
    /// <returns></returns>
        BaseResult CreateReportValidator(Report report, User user);
    }
}
