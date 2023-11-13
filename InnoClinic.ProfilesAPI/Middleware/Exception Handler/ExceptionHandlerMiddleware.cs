using Domain.Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using ILogger = Serilog.ILogger;

namespace InnoClinic.ProfilesAPI.Middleware.Exception_Handler
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception has been thrown : {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                DbUpdateException => (int)HttpStatusCode.BadRequest,
                SecurityTokenSignatureKeyNotFoundException => (int)HttpStatusCode.Unauthorized,
                ValidationException => (int)HttpStatusCode.BadRequest,
                ProfileNotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };
            await context.Response.WriteAsync(new ErrorDetails()
            {
                Message = exception switch
                {
                    DbUpdateException => "Invalid database operation (are you trying to create existing resource?)",
                    SecurityTokenSignatureKeyNotFoundException => "Invalid authorization token",
                    ValidationException => "Invalid model",
                    ProfileNotFoundException => "Profile not found",
                    _ => "Internal server error"
                }
            }.ToString());
        }
    }
}
