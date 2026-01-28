using FairWorkly.Domain.Common;
using FluentValidation;
using MediatR;

namespace FairWorkly.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IResultBase // Key constraint: only applies to Result-based handlers
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        // If no validators are defined, skip validation and continue
        if (!_validators.Any())
        {
            return await next();
        }

        // Create a validation context
        var context = new ValidationContext<TRequest>(request);

        // Execute all validators in parallel
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        // Collect all failures
        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToList();

        // If there are failures, return Result.ValidationFailure (no exception thrown!)
        if (failures.Any())
        {
            var errors = failures
                .Select(f => new ValidationError
                {
                    Field = f.PropertyName,
                    Message = f.ErrorMessage,
                })
                .ToList();

            // Use reflection to create the corresponding Result<T> type
            var resultType = typeof(TResponse).GetGenericArguments()[0];
            var method = typeof(Result<>)
                .MakeGenericType(resultType)
                .GetMethod(
                    nameof(Result<object>.ValidationFailure),
                    new[] { typeof(List<ValidationError>) }
                );

            return (TResponse)method!.Invoke(null, new object[] { errors })!;
        }

        // Validation passed — continue to the next pipeline step (the handler)
        return await next();
    }
}
