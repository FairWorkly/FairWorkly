namespace FairWorkly.Domain.Common;

/// <summary>
/// Marker interface for Result types.
/// Used by <c>ValidationBehavior</c> as a generic constraint
/// (<c>where TResponse : IResultBase</c>) to identify Result-based handlers.
/// </summary>
public interface IResultBase
{
    /// <summary>HTTP status code.</summary>
    int Code { get; }

    /// <summary>Whether the result represents a successful outcome (2xx).</summary>
    bool IsSuccess { get; }

    /// <summary>Human-readable summary message.</summary>
    string? Message { get; }

    /// <summary>Structured error details (stored as <c>object?</c>).</summary>
    object? Errors { get; }
}
