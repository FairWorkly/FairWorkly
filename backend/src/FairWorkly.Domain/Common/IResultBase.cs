namespace FairWorkly.Domain.Common;

/// <summary>
/// Marker interface for Result types.
/// Used by <c>ValidationBehavior</c> as a generic constraint
/// (<c>where TResponse : IResultBase</c>) to identify Result-based handlers.
/// </summary>
public interface IResultBase
{
    // ── New API ──

    /// <summary>HTTP status code.</summary>
    int Code { get; }

    /// <summary>Whether the result represents a successful outcome (2xx).</summary>
    bool IsSuccess { get; }

    /// <summary>Human-readable summary message.</summary>
    string? Message { get; }

    /// <summary>Structured error details (stored as <c>object?</c>).</summary>
    object? Errors { get; }

    // ── Obsolete (bridge — will be removed) ──

    /// <inheritdoc cref="IsSuccess"/>
    [Obsolete("Use !IsSuccess instead.")]
    bool IsFailure { get; }

    /// <inheritdoc cref="Message"/>
    [Obsolete("Use Message instead.")]
    string? ErrorMessage { get; }

    /// <inheritdoc cref="Errors"/>
    [Obsolete("Use Errors instead. Cast to the expected list type.")]
    List<ValidationError>? ValidationErrors { get; }

    /// <inheritdoc cref="Code"/>
    [Obsolete("Use Code (int) instead. ResultType enum will be removed.")]
    ResultType Type { get; }
}
