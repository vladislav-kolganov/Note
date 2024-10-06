using Note.Application.Resources;
using Note.Domain.Entity;
using Note.Domain.Enum;
using Note.Domain.Interfaces.Validations;
using Note.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Application.Validations
{
    public class ReportValidator : IReportValidator
    {
        public BaseResult CreateReportValidator(Report report, User user)
        {
            if (report != null)
            {
                return new BaseResult
                {
                    ErrorMessage = ErrorMessage.ReportAlreadyExists,
                    ErrorCode = (int)ErrorCodes.ReportAlreadyExists
                };
            }
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

        public BaseResult ValidateOnNull(Report model)
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
}
