namespace FairWorkly.Domain.Common;

/// <summary>
/// Marker interface for Result types, used for ValidationBehavior's generic constraint
/// </summary>
public interface IResultBase
{
    bool IsSuccess { get; }
    bool IsFailure { get; }
    string? ErrorMessage { get; }
    List<ValidationError>? ValidationErrors { get; }
    ResultType Type { get; }
}
