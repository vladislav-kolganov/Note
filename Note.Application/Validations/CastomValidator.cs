using Note.Application.Resources;
using Note.Domain.Entity;
using Note.Domain.Enum;
using Note.Domain.Interfaces.Validations;
using Note.Domain.Result;

namespace Note.Application.Validations;

public class CastomValidator : ICastomValidator
{
    public BaseResult UserExistValidator(User user)
    {
        if (user == null)
        {
            return new BaseResult
            {
                ErrorMessage = ErrorMessage.UserNotFound,
                ErrorCode = (int)ErrorCodes.UserNotFound

            };
        }

        return new BaseResult();
    }

    public BaseResult ValidateReportOnNull(Report model)
    {
        if (model == null)
        {
            return new BaseResult
            {
                ErrorMessage = ErrorMessage.ReportNotFound,
                ErrorCode = (int)ErrorCodes.ReportNotFound
            };
        }

        return new BaseResult();
    }
}