namespace FairWorkly.Domain.Payroll;

/// <summary>
/// Structured error for CSV parsing and format validation failures (HTTP 422).
/// Each instance represents one field-level error in a specific CSV row.
/// </summary>
/// <remarks>
/// This class is standalone — it does not inherit from any base error class.
/// The fields are dictated by the API contract, not by a shared type hierarchy.
/// Use with <c>Result&lt;T&gt;.Of422("message", errors)</c>.
/// </remarks>
public class Csv422Error
{
    /// <summary>The 1-based row number in the CSV file where the error occurred.</summary>
    public int RowNumber { get; init; }

    /// <summary>The field name that failed validation (maps to JSON <c>field</c>).</summary>
    public required string Field { get; init; }

    /// <summary>Human-readable error message for the frontend (maps to JSON <c>message</c>).</summary>
    public required string Message { get; init; }
}
