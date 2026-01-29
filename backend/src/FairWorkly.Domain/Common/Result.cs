namespace FairWorkly.Domain.Common;

public class Result<T> : IResultBase
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public string? ErrorMessage { get; }
    public List<ValidationError>? ValidationErrors { get; }
    public ResultType Type { get; }

    private Result(
        bool isSuccess,
        T? value,
        string? errorMessage,
        List<ValidationError>? validationErrors,
        ResultType type
    )
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = errorMessage;
        ValidationErrors = validationErrors;
        Type = type;
    }

    public static Result<T> Success(T value) => new(true, value, null, null, ResultType.Success);

    public static Result<T> ValidationFailure(List<ValidationError> errors) =>
        new(false, default, "Validation failed", errors, ResultType.ValidationFailure);

    public static Result<T> ValidationFailure(string errorMessage, List<ValidationError> errors) =>
        new(false, default, errorMessage, errors, ResultType.ValidationFailure);

    public static Result<T> Failure(string message) =>
        new(false, default, message, null, ResultType.BusinessFailure);

    public static Result<T> NotFound(string message) =>
        new(false, default, message, null, ResultType.NotFound);

    public static Result<T> Unauthorized(string message) =>
        new(false, default, message, null, ResultType.Unauthorized);

    public static Result<T> Forbidden(string message) =>
        new(false, default, message, null, ResultType.Forbidden);
}

public enum ResultType
{
    Success,
    ValidationFailure,
    BusinessFailure,
    NotFound,
    Unauthorized,
    Forbidden,
}
