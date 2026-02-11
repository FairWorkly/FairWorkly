using FairWorkly.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FairWorkly.API.Filters;

/// <summary>
/// Globally intercepts ObjectResult responses containing Result&lt;T&gt; values
/// and transforms them into appropriate HTTP responses with ProblemDetails for errors.
///
/// Controllers signal the filter by returning: new ObjectResult(result)
/// where result implements IResultBase.
///
/// For success cases, the filter unwraps Result&lt;T&gt;.Value and returns 200 OK.
/// For failure cases, the filter maps ResultType to the appropriate HTTP status code
/// with a ProblemDetails body consistent with GlobalExceptionHandler.
/// </summary>
public class ResultMappingFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var executedContext = await next();

        if (executedContext.Result is not ObjectResult objectResult)
            return;

        if (objectResult.Value is not IResultBase result)
            return;

        if (result.IsSuccess)
        {
            var value = objectResult.Value
                .GetType()
                .GetProperty(nameof(Result<object>.Value))
                ?.GetValue(objectResult.Value);

            executedContext.Result = new OkObjectResult(value);
            return;
        }

        executedContext.Result = MapFailureToActionResult(result, context.HttpContext);
    }

    private static ObjectResult MapFailureToActionResult(
        IResultBase result,
        HttpContext httpContext)
    {
        var (statusCode, title) = result.Type switch
        {
            ResultType.ValidationFailure => (StatusCodes.Status400BadRequest, "Validation Failed"),
            ResultType.BusinessFailure => (StatusCodes.Status400BadRequest, "Bad Request"),
            ResultType.NotFound => (StatusCodes.Status404NotFound, "Resource Not Found"),
            ResultType.Unauthorized => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            ResultType.Forbidden => (StatusCodes.Status403Forbidden, "Forbidden"),
            _ => (StatusCodes.Status400BadRequest, "Bad Request"),
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = result.Type == ResultType.ValidationFailure
                ? "One or more validation errors occurred."
                : result.ErrorMessage,
            Instance = httpContext.Request.Path,
        };

        if (result.Type == ResultType.ValidationFailure
            && result.ValidationErrors is { Count: > 0 })
        {
            var errors = result.ValidationErrors
                .GroupBy(e => e.Field)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToArray());

            problemDetails.Extensions.Add("errors", errors);
        }

        return new ObjectResult(problemDetails) { StatusCode = statusCode };
    }
}
