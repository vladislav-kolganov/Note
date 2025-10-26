using FluentValidation;
using Note.Domain.Dto.ReportDto;

namespace Note.Application.Validations.FluentValidations.Report;

public class CreateReportValidator : AbstractValidator<CreateReportDto>
{
    /// <summary>
    /// Валидатора для создания отчета.
    /// </summary>
    public CreateReportValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
    }
}
