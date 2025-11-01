using Note.API;
using Note.API.Hubs;
using Note.API.Middlewares;
using Note.Application.DependencyInjection;
using Note.DAL.DependencyInjection;
using Prometheus;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromMinutes(2);
});

builder.Services.AddAuthenticationAndAutorization(builder);
builder.Services.AddSwagger();

builder.Services.AddHttpContextAccessor();

builder.Services.UseHttpClientMetrics();

builder.Services.AddDataAccessLayer(builder.Configuration);

builder.Configuration.AddEnvironmentVariables();

builder.Logging.ClearProviders();

builder.Host.UseSerilog((context, config) =>
{
    config.Enrich.FromLogContext().
    Enrich.WithMachineName().
    WriteTo.Console().
    WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(context.Configuration["ElkSettings:Uri"]))
    {
        IndexFormat = $"{context.Configuration["ApplicationName:Name"]}-logs-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
        AutoRegisterTemplate = true
    }).
    Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName).
    ReadFrom.Configuration(context.Configuration);

});

builder.Services.AddApplication();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(op =>
    {
        op.SwaggerEndpoint("/swagger/v1/swagger.json", "Note Swagger v 1.0");
        op.RoutePrefix = string.Empty;
    });
}
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseMetricServer();
app.UseHttpMetrics();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.Run();
