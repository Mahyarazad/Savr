using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Savr.Presentation.Helpers;

public static class ControllerActionExecutor
{
    public static async Task<IActionResult> SafeExecute<TResponse>(
        Func<Task<Result<TResponse>>> handler,
        Func<TResponse, IActionResult>? onSuccess = null)
    {
        try
        {
            var result = await handler();

            if (result.IsSuccess)
            {
                return onSuccess != null
                    ? onSuccess(result.Value)
                    : new OkObjectResult(result.Value);
            }

            return new BadRequestObjectResult(ResultErrorParser.ParseResultError(result.Errors));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error occurred in controller action.");
            return new ObjectResult(new { error = "An unexpected error occurred." })
            {
                StatusCode = 500
            };
        }
    }
}
