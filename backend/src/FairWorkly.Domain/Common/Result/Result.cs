namespace FairWorkly.Domain.Common.Result;

/// <summary>
/// Unified result type for all Handler return values.
/// Use static <c>Of{code}</c> factory methods to create instances.
/// </summary>
/// <remarks>
/// <para>
/// Result&lt;T&gt; is a "fill-in-the-blank" pattern — pick a code, write a message, provide errors (if any).
/// The HTTP status code is baked into the factory method name, so you never pick the wrong one.
/// </para>
/// <para>
/// <b>Internal design:</b> The class stores <c>int Code</c> directly (no enum).
/// <c>IsSuccess</c> is derived: <c>Code is &gt;= 200 and &lt; 300</c>.
/// Errors are stored as <c>object?</c> — Result is a carrier, it does not inspect error contents.
/// Type safety for errors is enforced at the factory method level via method-level generics.
/// </para>
/// </remarks>
/// <typeparam name="T">The type of the success value (DTO). Use <c>Unit</c> for operations with no return value.</typeparam>
public class Result<T> : IResultBase
{
    // ════════════════════════════════════════════════════════════════
    // Properties
    // ════════════════════════════════════════════════════════════════

    /// <summary>HTTP status code (e.g., 200, 400, 404, 422). Set by the factory method.</summary>
    public int Code { get; }

    /// <summary>Whether the result represents a successful outcome (2xx).</summary>
    public bool IsSuccess => Code is >= 200 and < 300;

    /// <summary>The success value. Only meaningful when <see cref="IsSuccess"/> is true.</summary>
    public T? Value { get; }

    /// <summary>
    /// Human-readable summary message for the frontend.
    /// All codes carry a message — some have sensible defaults (401, 403, 404, 409),
    /// others require an explicit message (200, 201, 422).
    /// </summary>
    public string? Message { get; }

    /// <summary>
    /// Structured error details (e.g., <c>List&lt;Validation400Error&gt;</c>, <c>List&lt;Csv422Error&gt;</c>).
    /// Stored as <c>object?</c> — Result is a carrier, it does not inspect the contents.
    /// Only present for codes that carry structured errors (400, 422).
    /// </summary>
    public object? Errors { get; }

    // ════════════════════════════════════════════════════════════════
    // Private constructor — all instances created via factory methods
    // ════════════════════════════════════════════════════════════════

    private Result(int code, T? value, string? message, object? errors)
    {
        Code = code;
        Value = value;
        Message = message;
        Errors = errors;
    }

    // ════════════════════════════════════════════════════════════════
    // Of{code} factory methods
    // ════════════════════════════════════════════════════════════════

    // ── 200 OK ──────────────────────────────────────────────────────

    /// <summary>Creates a <b>200 OK</b> result.</summary>
    /// <param name="message">Frontend-facing summary (e.g., "Audit completed successfully").</param>
    /// <param name="value">The DTO to return.</param>
    /// <example><code>return Result&lt;PayrollDto&gt;.Of200("Payroll validation completed", dto);</code></example>
    public static Result<T> Of200(string message, T value) => new(200, value, message, null);

    // ── 201 Created ─────────────────────────────────────────────────

    /// <summary>Creates a <b>201 Created</b> result.</summary>
    /// <param name="message">Frontend-facing summary (e.g., "Employee created").</param>
    /// <param name="value">The DTO of the newly created resource.</param>
    public static Result<T> Of201(string message, T value) => new(201, value, message, null);

    // ── 204 No Content ──────────────────────────────────────────────

    /// <summary>Creates a <b>204 No Content</b> result. Typically used for delete operations.</summary>
    /// <remarks>Use with <c>Result&lt;Unit&gt;</c> since there is no response body.</remarks>
    /// <example><code>return Result&lt;Unit&gt;.Of204();</code></example>
    public static Result<T> Of204() => new(204, default, null, null);

    // ── 400 Bad Request (ValidationBehavior exclusive) ──────────────

    /// <summary>
    /// Creates a <b>400 Bad Request</b> result with the default message "Request validation failed".
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method is called exclusively by <c>ValidationBehavior</c> via reflection.
    /// Handlers should <b>never</b> call this directly — FluentValidation + ValidationBehavior
    /// handle 400 automatically. If your request reaches the Handler, it already passed validation.
    /// </para>
    /// <para>
    /// Reflection call site: <c>ValidationBehavior.cs</c> uses <c>GetMethod("Of400", ...)</c>
    /// to find this concrete overload by matching the <c>List&lt;Validation400Error&gt;</c> parameter type.
    /// </para>
    /// </remarks>
    /// <param name="errors">Validation errors produced by FluentValidation.</param>
    public static Result<T> Of400(List<Validation400Error> errors) =>
        new(400, default, "Request validation failed", errors);

    /// <summary>Creates a <b>400 Bad Request</b> result with a custom message and typed error list.</summary>
    /// <typeparam name="TError">Error type — inferred by the compiler from the list you pass.</typeparam>
    public static Result<T> Of400<TError>(string message, List<TError> errors) =>
        new(400, default, message, errors);

    // ── 401 Unauthorized ────────────────────────────────────────────

    /// <summary>Creates a <b>401 Unauthorized</b> result with the default message "Unauthorized".</summary>
    public static Result<T> Of401() => new(401, default, "Unauthorized", null);

    /// <summary>Creates a <b>401 Unauthorized</b> result with a custom message.</summary>
    public static Result<T> Of401(string message) => new(401, default, message, null);

    // ── 403 Forbidden ───────────────────────────────────────────────

    /// <summary>Creates a <b>403 Forbidden</b> result with the default message "Forbidden".</summary>
    public static Result<T> Of403() => new(403, default, "Forbidden", null);

    /// <summary>Creates a <b>403 Forbidden</b> result with a custom message.</summary>
    /// <example><code>return Result&lt;PayrollDto&gt;.Of403("User does not belong to an organization");</code></example>
    public static Result<T> Of403(string message) => new(403, default, message, null);

    // ── 404 Not Found ───────────────────────────────────────────────

    /// <summary>Creates a <b>404 Not Found</b> result with the default message "Not found".</summary>
    public static Result<T> Of404() => new(404, default, "Not found", null);

    /// <summary>Creates a <b>404 Not Found</b> result with a custom message.</summary>
    public static Result<T> Of404(string message) => new(404, default, message, null);

    // ── 409 Conflict ────────────────────────────────────────────────

    /// <summary>Creates a <b>409 Conflict</b> result with the default message "Conflict".</summary>
    public static Result<T> Of409() => new(409, default, "Conflict", null);

    /// <summary>Creates a <b>409 Conflict</b> result with a custom message.</summary>
    /// <example><code>return Result&lt;EmployeeDto&gt;.Of409("Employee number E999 already exists");</code></example>
    public static Result<T> Of409(string message) => new(409, default, message, null);

    // ── 422 Unprocessable Entity ────────────────────────────────────

    /// <summary>
    /// Creates a <b>422 Unprocessable Entity</b> result with a message only (no structured errors).
    /// Used when processing fails but there are no field-level error details to report.
    /// </summary>
    /// <param name="message">Frontend-facing summary (e.g., "Roster validation failed.").</param>
    /// <example><code>return Result&lt;RosterDto&gt;.Of422("Roster validation failed.");</code></example>
    public static Result<T> Of422(string message) => new(422, default, message, null);

    /// <summary>
    /// Creates a <b>422 Unprocessable Entity</b> result with a message and structured error list.
    /// Used for business-level validation failures (CSV parsing, format checks, etc.).
    /// </summary>
    /// <typeparam name="TError">Error type — inferred by the compiler from the list you pass.</typeparam>
    /// <param name="message">Frontend-facing summary (e.g., "CSV format validation failed").</param>
    /// <param name="errors">Structured error list (e.g., <c>List&lt;Csv422Error&gt;</c>).</param>
    /// <example><code>return Result&lt;PayrollDto&gt;.Of422("CSV file parsing failed", errors);</code></example>
    public static Result<T> Of422<TError>(string message, List<TError> errors) =>
        new(422, default, message, errors);

    // ── 500 Internal Server Error ─────────────────────────────────────

    /// <summary>
    /// Creates a <b>500 Internal Server Error</b> result with a user-facing message.
    /// Used when a Handler catches an anticipated infrastructure failure
    /// (external service down, storage unavailable, database error)
    /// and wants to return a friendly message instead of letting the exception
    /// propagate to GlobalExceptionHandler.
    /// </summary>
    /// <param name="message">Frontend-facing summary (e.g., "Failed to save roster. Please try again or contact support.").</param>
    /// <example><code>return Result&lt;RosterDto&gt;.Of500("Failed to save roster. Please try again or contact support.");</code></example>
    public static Result<T> Of500(string message) => new(500, default, message, null);
}
