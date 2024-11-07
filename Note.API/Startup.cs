using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Note.Domain.Settings;
using System.Reflection;
using System.Text;

namespace Note.API
{
    public static class Startup
    {
     
        /// <summary>
        /// Подключение аутентификации и авторизации
        /// </summary>
        /// <param name="services"></param>
        public static void AddAuthenticationAndAutorization(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddAuthorization();
            services.AddAuthentication(options =>
            {

                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(op => 
            
                {
                    var options = builder.Configuration.GetSection(JwtSettings.DefaultSection).Get<JwtSettings>();       
                    var jwtKey = options.JwtKey;
                    var issuer = options.Issuer;
                    var audience = options.Audience;
                    op.Authority = options.Authority;
                    op.RequireHttpsMetadata = false;
                    op.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes (jwtKey)),
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };
                });

        }
        /// <summary>
        /// Подключение Swager
        /// </summary>
        /// <param name="services"></param>
        public static void AddSwagger(this IServiceCollection services)
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

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));// подключение комментариев 
            });


        }
    }
}
