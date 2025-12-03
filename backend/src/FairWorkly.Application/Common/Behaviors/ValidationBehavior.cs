using FluentValidation;
using MediatR;

namespace FairWorkly.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
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

        // If there are failures, throw FluentValidation's ValidationException
        // (this will short-circuit the pipeline and the handler will not run)
        if (failures.Any())
        {
            throw new ValidationException(failures);
        }

        // Validation passed — continue to the next pipeline step (the handler)
        return await next();
    }
}
