using Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Exceptions;

public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails = exception switch
        {
            ValidationException ex => new ValidationProblemDetails(
                ex.Errors.GroupBy(e => e.PropertyName)
                         .ToDictionary(g => g.Key,
                                       g => g.Select(e => e.ErrorMessage).ToArray()
                         )
            )
            {
                Title = "Validation failed",
                Status = StatusCodes.Status400BadRequest
            },
                

            NotFoundException ex => new ProblemDetails
            {
                Title = "Resource not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            },
            _ => new ProblemDetails
            {
                Title = "An unexpected error occurred.",
                Detail = "An unhandled exception!",
                Status = StatusCodes.Status500InternalServerError
            }
        };

        httpContext.Response.StatusCode = problemDetails.Status!.Value;

        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });

        return true;

    }
}
