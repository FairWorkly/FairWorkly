namespace FairWorkly.Domain.Common;

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
    // Properties (new API)
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
    // Obsolete properties (bridge to new API — will be removed)
    // ════════════════════════════════════════════════════════════════

    /// <inheritdoc />
    [Obsolete("Use !IsSuccess instead.")]
    public bool IsFailure => !IsSuccess;

    /// <inheritdoc />
    [Obsolete("Use Message instead.")]
    public string? ErrorMessage => Message;

    /// <inheritdoc />
    [Obsolete("Use Errors instead. Cast to the expected list type.")]
    public List<ValidationError>? ValidationErrors => Errors as List<ValidationError>;

    /// <inheritdoc />
    [Obsolete("Use Code (int) instead. ResultType enum will be removed.")]
    public ResultType Type => Code switch
    {
        200 => ResultType.Success,
        201 => ResultType.Success,
        204 => ResultType.Success,
        400 => ResultType.ValidationFailure,
        401 => ResultType.Unauthorized,
        403 => ResultType.Forbidden,
        404 => ResultType.NotFound,
        409 => ResultType.BusinessFailure,
        422 => ResultType.ProcessingFailure,
        _ => ResultType.Success,
    };

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
    // New API: Of{code} factory methods
    // ════════════════════════════════════════════════════════════════

    // ── 200 OK ──────────────────────────────────────────────────────

    /// <summary>Creates a <b>200 OK</b> result.</summary>
    /// <param name="message">Frontend-facing summary (e.g., "Audit completed successfully").</param>
    /// <param name="value">The DTO to return.</param>
    /// <example><code>return Result&lt;PayrollDto&gt;.Of200("Payroll validation completed", dto);</code></example>
    public static Result<T> Of200(string message, T value) =>
        new(200, value, message, null);

    // ── 201 Created ─────────────────────────────────────────────────

    /// <summary>Creates a <b>201 Created</b> result.</summary>
    /// <param name="message">Frontend-facing summary (e.g., "Employee created").</param>
    /// <param name="value">The DTO of the newly created resource.</param>
    public static Result<T> Of201(string message, T value) =>
        new(201, value, message, null);

    // ── 204 No Content ──────────────────────────────────────────────

    /// <summary>Creates a <b>204 No Content</b> result. Typically used for delete operations.</summary>
    /// <remarks>Use with <c>Result&lt;Unit&gt;</c> since there is no response body.</remarks>
    /// <example><code>return Result&lt;Unit&gt;.Of204();</code></example>
    public static Result<T> Of204() =>
        new(204, default, null, null);

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
    /// to find this concrete overload by matching the <c>List&lt;ValidationError&gt;</c> parameter type.
    /// </para>
    /// </remarks>
    /// <param name="errors">Validation errors produced by FluentValidation.</param>
    public static Result<T> Of400(List<ValidationError> errors) =>
        new(400, default, "Request validation failed", errors);

    /// <summary>Creates a <b>400 Bad Request</b> result with a custom message and typed error list.</summary>
    /// <typeparam name="TError">Error type — inferred by the compiler from the list you pass.</typeparam>
    public static Result<T> Of400<TError>(string message, List<TError> errors) =>
        new(400, default, message, errors);

    // ── 401 Unauthorized ────────────────────────────────────────────

    /// <summary>Creates a <b>401 Unauthorized</b> result with the default message "Unauthorized".</summary>
    public static Result<T> Of401() =>
        new(401, default, "Unauthorized", null);

    /// <summary>Creates a <b>401 Unauthorized</b> result with a custom message.</summary>
    public static Result<T> Of401(string message) =>
        new(401, default, message, null);

    // ── 403 Forbidden ───────────────────────────────────────────────

    /// <summary>Creates a <b>403 Forbidden</b> result with the default message "Forbidden".</summary>
    public static Result<T> Of403() =>
        new(403, default, "Forbidden", null);

    /// <summary>Creates a <b>403 Forbidden</b> result with a custom message.</summary>
    /// <example><code>return Result&lt;PayrollDto&gt;.Of403("User does not belong to an organization");</code></example>
    public static Result<T> Of403(string message) =>
        new(403, default, message, null);

    // ── 404 Not Found ───────────────────────────────────────────────

    /// <summary>Creates a <b>404 Not Found</b> result with the default message "Not found".</summary>
    public static Result<T> Of404() =>
        new(404, default, "Not found", null);

    /// <summary>Creates a <b>404 Not Found</b> result with a custom message.</summary>
    public static Result<T> Of404(string message) =>
        new(404, default, message, null);

    // ── 409 Conflict ────────────────────────────────────────────────

    /// <summary>Creates a <b>409 Conflict</b> result with the default message "Conflict".</summary>
    public static Result<T> Of409() =>
        new(409, default, "Conflict", null);

    /// <summary>Creates a <b>409 Conflict</b> result with a custom message.</summary>
    /// <example><code>return Result&lt;EmployeeDto&gt;.Of409("Employee number E999 already exists");</code></example>
    public static Result<T> Of409(string message) =>
        new(409, default, message, null);

    // ── 422 Unprocessable Entity ────────────────────────────────────

    /// <summary>
    /// Creates a <b>422 Unprocessable Entity</b> result with a message only (no structured errors).
    /// Used when processing fails but there are no field-level error details to report.
    /// </summary>
    /// <param name="message">Frontend-facing summary (e.g., "Roster validation failed.").</param>
    /// <example><code>return Result&lt;RosterDto&gt;.Of422("Roster validation failed.");</code></example>
    public static Result<T> Of422(string message) =>
        new(422, default, message, null);

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

    // ════════════════════════════════════════════════════════════════
    // Obsolete factory methods (bridge — will be removed in cleanup)
    // ════════════════════════════════════════════════════════════════

    /// <inheritdoc cref="Of200"/>
    [Obsolete("Use Of200(message, value) instead.")]
    public static Result<T> Success(T value) =>
        new(200, value, "Success", null);

    /// <inheritdoc cref="Of400(List{ValidationError})"/>
    [Obsolete("Use Of400 instead. This method is only called by ValidationBehavior.")]
    public static Result<T> ValidationFailure(List<ValidationError> errors) =>
        Of400(errors);

    /// <inheritdoc cref="Of400{TError}(string, List{TError})"/>
    [Obsolete("Use Of400 instead.")]
    public static Result<T> ValidationFailure(string errorMessage, List<ValidationError> errors) =>
        new(400, default, errorMessage, errors);

    /// <summary>Obsolete. BusinessFailure has been removed — use the appropriate Of{code} method.</summary>
    [Obsolete("BusinessFailure removed. Use the appropriate Of{code} method.")]
    public static Result<T> Failure(string message) =>
        new(422, default, message, null);

    /// <inheritdoc cref="Of422{TError}"/>
    [Obsolete("Use Of422<TError>(message, errors) instead.")]
    public static Result<T> ProcessingFailure(string message, List<ValidationError> errors) =>
        new(422, default, message, errors);

    /// <inheritdoc cref="Of404(string)"/>
    [Obsolete("Use Of404() or Of404(message) instead.")]
    public static Result<T> NotFound(string message) =>
        Of404(message);

    /// <inheritdoc cref="Of401(string)"/>
    [Obsolete("Use Of401() or Of401(message) instead.")]
    public static Result<T> Unauthorized(string message) =>
        Of401(message);

    /// <inheritdoc cref="Of403(string)"/>
    [Obsolete("Use Of403() or Of403(message) instead.")]
    public static Result<T> Forbidden(string message) =>
        Of403(message);
}

/// <summary>Obsolete. Use <c>Result&lt;T&gt;.Code</c> (int) directly. This enum will be removed.</summary>
[Obsolete("Use Result<T>.Code (int) instead. This enum will be removed.")]
public enum ResultType
{
    Success,
    ValidationFailure,
    BusinessFailure,
    ProcessingFailure,
    NotFound,
    Unauthorized,
    Forbidden,
}
