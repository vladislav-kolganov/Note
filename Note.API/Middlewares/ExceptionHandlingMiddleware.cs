using Note.Domain.Enum;
using Note.Domain.Result;
using System.Diagnostics;
using System.Net;
namespace Note.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, Serilog.ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                Debugger.Break();

                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            _logger.Error(exception, exception.Message);

            var errorMessage = exception.Message;
            var response = exception switch
            {
                UnauthorizedAccessException => new BaseResult() { ErrorMessage = errorMessage, ErrorCode = (int)HttpStatusCode.Unauthorized },
                _ => new BaseResult() { ErrorMessage = "Internal server error. Please retry later", ErrorCode = (int)HttpStatusCode.InternalServerError }
            };

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)response.ErrorCode;
            await httpContext.Response.WriteAsJsonAsync(response);
        }
    }

}
