namespace FairWorkly.Domain.Common;

/// <summary>
/// Generic validation error base class containing only basic field information.
/// Business modules can inherit from this class to add business-specific fields.
/// </summary>
public class ValidationError
{
    public string Field { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}
