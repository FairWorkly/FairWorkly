namespace FairWorkly.Domain.Common;

/// <summary>
/// Generic validation error base class containing only basic field information.
/// Business modules can inherit from this class to add business-specific fields.
/// </summary>
public class ValidationError
{
    public required string Field { get; init; }
    public required string Message { get; init; }
}
