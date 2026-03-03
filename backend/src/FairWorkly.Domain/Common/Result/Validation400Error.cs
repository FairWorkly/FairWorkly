namespace FairWorkly.Domain.Common.Result;

/// <summary>
/// Validation error produced by FluentValidation and returned via <c>Result&lt;T&gt;.Of400</c>.
/// Each error represents a single field-level validation failure.
/// </summary>
public class Validation400Error
{
    public required string Field { get; init; }
    public required string Message { get; init; }
}
