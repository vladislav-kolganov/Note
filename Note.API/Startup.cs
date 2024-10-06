using Asp.Versioning;
using Microsoft.OpenApi.Models;

namespace Note.API
{
    public static class Startup
    {
        /// <summary>
        /// Подключение Swager
        /// </summary>
        /// <param name="services"></param>
        public static void AddSwager(this IServiceCollection services)
        {
            services.AddApiVersioning()
            .AddApiExplorer(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
                options.AssumeDefaultVersionWhenUnspecified = true; // если не задаётся версия, то используется по умолчанию
            });
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo()
                {

                    Version = "v1",
                    Title = "Note.API",
                    Description = "This is version 1.0"

                });

                options.SwaggerDoc("v2", new OpenApiInfo()
                {

                    Version = "v2",
                    Title = "Note.API",
                    Description = "This is version 2.0"

                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {

                    In = ParameterLocation.Header,
                    Description = "Введите пожалуйста валидный токен",
                    Type = SecuritySchemeType.Http,
                    Name = "Авторизация",
                    BearerFormat = "JWT",
                    Scheme = "Bearer"

                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme ()
                        {
                             Reference= new OpenApiReference()
                             {
                             Type = ReferenceType.SecurityScheme,
                             Id = "Bearer"
                             }
                             ,Name = "Bearer",
                              In = ParameterLocation.Header
                        }
                        ,Array.Empty<string>()
                    }
                });

            });


        }
    }
}
