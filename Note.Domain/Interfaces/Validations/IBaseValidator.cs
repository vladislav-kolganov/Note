using Note.Domain.Result;

namespace Note.Domain.Interfaces.Validations;

public interface IBaseValidator<in T> where T : class
{
    BaseResult ValidateReportOnNull(T model);
}
