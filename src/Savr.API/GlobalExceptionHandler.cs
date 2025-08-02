using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Savr.API
{
    internal sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;
        public GlobalExceptionHandler(IProblemDetailsService problemDetailsService)
        {
            _problemDetailsService = problemDetailsService;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            Log.Error(exception, "An unhandled exception occurred.");

            httpContext.Response.StatusCode =exception switch
            {
                ArgumentNullException => StatusCodes.Status400BadRequest,
                ArgumentException => StatusCodes.Status400BadRequest,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            return await _problemDetailsService.TryWriteAsync(
               new ProblemDetailsContext
               {
                   HttpContext = httpContext,
                   Exception = exception,
                   ProblemDetails = new ProblemDetails
                   {
                       Title = "An error occurred while processing your request.",
                       Detail = exception.Message,
                       Status = httpContext.Response.StatusCode,
                       Type = "https://httpstatuses.org/" + httpContext.Response.StatusCode
                   }
               }
            );


        }
    }
}
