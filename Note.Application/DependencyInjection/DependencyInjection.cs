using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Note.Application.Mapping;
using Note.Application.Services;
using Note.Application.Validations;
using Note.Application.Validations.FluentValidations.Report;
using Note.Domain.Dto.ReportDto;
using Note.Domain.Interfaces.Services;
using Note.Domain.Interfaces.Validations;

namespace Note.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ReportMapping));
            services.AddAutoMapper(typeof(UserMapping));
            InitServices(services);

            InitFluentValidation(services);
            InitEntityValidators(services);
        }
        public static void InitServices(this IServiceCollection services)
        {
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
        }

        public static void InitFluentValidation(this IServiceCollection services)
        {
            var validatorsTypes = new List<Type>()
            {
                typeof(CreateReportValidator),
                typeof(UpdateReportValidator)
            };

            foreach (var validatorType in validatorsTypes)
            {
                services.AddValidatorsFromAssembly(validatorType.Assembly);
            }
        }

        public static void InitEntityValidators(this IServiceCollection services)
        {
            services.AddScoped<IReportValidator, ReportValidator>();
        }

    }
}
