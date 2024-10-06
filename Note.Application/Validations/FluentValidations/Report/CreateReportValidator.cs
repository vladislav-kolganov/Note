using FluentValidation;
using Note.Domain.Dto.ReportDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Application.Validations.FluentValidations.Report
{
    public class CreateReportValidator : AbstractValidator<CreateReportDto>
    {
        public CreateReportValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);

        }
    }
}
