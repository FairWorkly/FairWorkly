using FairWorkly.Domain.Common;
using FluentValidation;
using MediatR;

namespace FairWorkly.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that intercepts requests before they reach the Handler
/// and runs FluentValidation validators. If any validation rule fails, returns a
/// <c>Result&lt;T&gt;.Of400</c> (HTTP 400) immediately — the Handler never executes.
/// </summary>
/// <remarks>
/// <para><b>How it works:</b></para>
/// <list type="number">
///   <item>MediatR sends a request (Command/Query) through the pipeline.</item>
///   <item>This behavior collects all <c>IValidator&lt;TRequest&gt;</c> registered via DI.</item>
///   <item>Runs all validators in parallel.</item>
///   <item>If any fail, maps <c>ValidationFailure</c> → <c>ValidationError</c> and
///         calls <c>Result&lt;T&gt;.Of400(errors)</c> via reflection.</item>
///   <item>If all pass, delegates to <c>next()</c> (the Handler).</item>
/// </list>
/// <para><b>Why reflection?</b></para>
/// <para>
/// <c>TResponse</c> is constrained to <c>IResultBase</c>, but the static factory method
/// <c>Of400</c> lives on <c>Result&lt;T&gt;</c> (a concrete generic class). We must use
/// reflection to call the correctly-typed <c>Result&lt;T&gt;.Of400(List&lt;ValidationError&gt;)</c>
/// at runtime, since <c>T</c> varies per handler.
/// </para>
/// <para><b>400 is exclusively owned by this behavior.</b></para>
/// <para>
/// Handlers should never call <c>Of400</c> directly. If a request reaches the Handler,
/// it has already passed validation. This ensures a clean separation: validators handle
/// input validation (400), handlers handle business logic (403, 404, 422, etc.).
/// </para>
/// </remarks>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IResultBase
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
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Any())
        {
            // Map FluentValidation failures to our ValidationError type.
            // Field = the property name (.OverridePropertyName() in validators controls this).
            // Message = the error message (.WithMessage() in validators controls this).
            var errors = failures
                .Select(f => new ValidationError
                {
                    Field = f.PropertyName,
                    Message = f.ErrorMessage,
                })
                .ToList();

            // Reflection: call Result<T>.Of400(List<ValidationError>) where T is the
            // handler's return DTO type. GetMethod finds the concrete (non-generic) overload
            // by matching the List<ValidationError> parameter type exactly.
            var resultType = typeof(TResponse).GetGenericArguments()[0];
            var method = typeof(Result<>)
                .MakeGenericType(resultType)
                .GetMethod(
                    nameof(Result<object>.Of400),
                    new[] { typeof(List<ValidationError>) }
                );

            return (TResponse)method!.Invoke(null, new object[] { errors })!;
        }

        return await next();
    }
}
