using FluentValidation;
using Note.Domain.Dto.ReportDto;

namespace Note.Application.Validations.FluentValidations.Report;

public class UpdateReportValidator : AbstractValidator<UpdateReportDto>
{
    /// <summary>
    /// Валидатор для обновления отчёта.
    /// </summary>
    public UpdateReportValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);

    }
}
