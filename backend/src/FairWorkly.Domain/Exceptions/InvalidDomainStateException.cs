namespace FairWorkly.Domain.Exceptions;

/// <summary>
/// Thrown when a domain entity is in an invalid state.
/// Used for guard clauses and invariant validation.
/// </summary>
public class InvalidDomainStateException : DomainException
{
    public string EntityName { get; }
    public string PropertyName { get; }

    public InvalidDomainStateException(string entityName, string propertyName, string message)
        : base($"{entityName}.{propertyName}: {message}")
    {
        EntityName = entityName;
        PropertyName = propertyName;
    }
}
