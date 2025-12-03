using FairWorkly.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        // Map exception type to HTTP status code and details
        var (statusCode, title, detail, extensions) = exception switch
        {
            // Validation Failure (400)
            ValidationException valEx => (
                StatusCodes.Status400BadRequest,
                "Validation Failed",
                "One or more validation errors occurred.",
                new Dictionary<string, object?>
                {
                    {
                        "errors",
                        valEx
                            .Errors.GroupBy(e => e.PropertyName)
                            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                    },
                }
            ),

            // Not Found (404)
            NotFoundException => (
                StatusCodes.Status404NotFound,
                "Resource Not Found",
                exception.Message,
                null
            ),

            // Forbidden (403)
            ForbiddenAccessException => (
                StatusCodes.Status403Forbidden,
                "Forbidden",
                exception.Message,
                null
            ),

            // Domain Rule Violation (422)
            DomainException => (
                StatusCodes.Status422UnprocessableEntity,
                "Business Rule Violation",
                exception.Message,
                null
            ),

            // Default / Internal Server Error (500)
            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An error occurred while processing your request.",
                null
            ),
        };

        // Log based on severity (Error for 5xx, Warning for 4xx)
        if (statusCode >= 500)
        {
            _logger.LogError(
                exception,
                "Unhandled exception occurred: {Message}",
                exception.Message
            );
        }
        else
        {
            _logger.LogWarning(
                "Application exception ({StatusCode}): {Message}",
                statusCode,
                exception.Message
            );
        }

        // Build RFC 7807 ProblemDetails response
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path,
        };

        // Add extension fields if available
        if (extensions != null)
        {
            foreach (var ext in extensions)
            {
                problemDetails.Extensions.Add(ext.Key, ext.Value);
            }
        }

        // Write response
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
