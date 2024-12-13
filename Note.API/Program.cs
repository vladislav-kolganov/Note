using Note.API;
using Note.API.Middlewares;
using Note.Application.DependencyInjection;
using Note.DAL.DependencyInjection;
using Note.Domain.Settings;
using Serilog;
using Note.Producer.DependencyInjection;
using Note.Consumer.DependencyInjection;
using Prometheus;

namespace Note.API
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(nameof(RabbitMqSettings)));
            builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection(nameof(RedisSettings)));
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.DefaultSection));
            // Add services to the container.

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.UseHttpClientMetrics();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddAuthenticationAndAutorization(builder);
            builder.Services.AddSwagger();

            builder.Services.AddHttpContextAccessor();

            builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration)); // берем конфигурацию из нашего appsettings.json

            builder.Services.AddDataAccessLayer(builder.Configuration); // регистрирование и добавление всех в зависимостей в слое DAL
            builder.Services.AddApplication(builder.Configuration);

            builder.Services.AddProducer();
            builder.Services.AddConsumer();


            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(op =>
                {
                    op.SwaggerEndpoint("/swagger/v1/swagger.json", "Note Swagger v 1.0");
                    op.SwaggerEndpoint("/swagger/v2/swagger.json", "Note Swagger v 2.0");
                    op.RoutePrefix = string.Empty;
                });
            }
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseHttpsRedirection();
            app.UseMetricServer();
            app.UseHttpMetrics();
            app.MapGet("/random-number", () =>
            {
                var number = Random.Shared.Next(0, 10);
                return Results.Ok(number);
            });
            app.MapMetrics();
            app.MapControllers();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}