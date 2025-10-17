using Microsoft.AspNetCore.Mvc;
using MyPinPad.Core.Exceptions;
using System.Text.Json;

namespace MyPinPad.WebApi.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (MyPinPadExceptionBase e)
            {
                _logger.LogError(e.InnerException, e.Message);
                await HandleExceptionAsync(httpContext, e);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode = exception switch
            {
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = statusCode;

            var problemDetails = new ProblemDetails
            {
                Title = exception.Message,
                Status = statusCode,
                Detail = _env.IsDevelopment() ? exception.StackTrace : null,
                Type = exception.GetType().Name,
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = statusCode;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(problemDetails, options);

            return context.Response.WriteAsync(json);
        }
    }
}
