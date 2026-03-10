using System.Net;
using System.Text.Json;
using Norden.API.DTOs.Common;

namespace Norden.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var statusCode = exception switch
            {
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                InvalidOperationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = statusCode;

            var errorCode = exception switch
            {
                UnauthorizedAccessException => "UNAUTHORIZED",
                KeyNotFoundException => "NOT_FOUND",
                InvalidOperationException => "INVALID_OPERATION",
                _ => "INTERNAL_SERVER_ERROR"
            };

            var message = _env.IsDevelopment() ? exception.Message : "An unexpected error occurred.";

            var response = ApiResponse<object>.ErrorResult(message, errorCode);
            
            // In development, include stack trace in the message if it's a 500
            if (_env.IsDevelopment() && statusCode == (int)HttpStatusCode.InternalServerError)
            {
                 response = ApiResponse<object>.ErrorResult(message, errorCode, new { StackTrace = exception.StackTrace });
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
