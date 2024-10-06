using Note.API;
using Note.Application.DependencyInjection;
using Note.DAL.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


builder.Services.AddSwager();

builder.Services.AddHttpContextAccessor();

builder.Host.UseSerilog((context, configuration) =>
configuration.ReadFrom.Configuration(context.Configuration)); // берем конфигурацию из нашего appsettings.json

builder.Services.AddDataAccessLayer(builder.Configuration); // регистрирование и добавление всех в зависимостей в слое DAL
builder.Services.AddApplication();

var app = builder.Build();

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

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
