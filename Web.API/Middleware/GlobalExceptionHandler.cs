using Application.Common.Exeptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;


namespace Web.API.Middleware
{
    public sealed class GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (status, title, detail) = exception switch
            {
                EntityNotFoundExeption => (
                    StatusCodes.Status404NotFound,
                    "Not Found",
                    exception.Message),
                EmailAlreadyRegisteredException => (
                    StatusCodes.Status400BadRequest,
                    "Bad Request",
                    exception.Message),
                UnauthorizedAccessException => (
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    exception.Message),
                _ => (
                    StatusCodes.Status500InternalServerError,
                    "Internal Server Error",
                    "An unexpected error occurred.")
            };

            if (status == StatusCodes.Status500InternalServerError)
                logger.LogError(exception, "Unhandled exception occurred.");
            else
                logger.LogWarning(exception, "Request failed: {Message}", exception.Message);

            context.Response.StatusCode = status;

            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = new ProblemDetails
                {
                    Status = status,
                    Title = title,
                    Detail = detail
                }
            });

            return true;
        }
    }
}

