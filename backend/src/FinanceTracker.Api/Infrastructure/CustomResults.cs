using FinanceTracker.SharedKernel;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Api.Infrastructure;

public static class CustomResults
{
    public static IActionResult Problem(Error error)
    {
        int statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        return new ObjectResult(new ProblemDetails
        {
            Title = GetTitle(error.Type),
            Status = statusCode,
            Extensions =
            {
                ["errors"] = error is ValidationError validationError
                    ? validationError.Errors
                    : new[] { error }
            }
        })
        {
            StatusCode = statusCode
        };
    }

    private static string GetTitle(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => "Bad Request",
            ErrorType.NotFound => "Not Found",
            ErrorType.Conflict => "Conflict",
            ErrorType.Unauthorized => "Unauthorized",
            _ => "Internal Server Error"
        };
}
