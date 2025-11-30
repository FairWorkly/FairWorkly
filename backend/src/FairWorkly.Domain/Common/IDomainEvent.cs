namespace FairWorkly.Domain.Common;

/// <summary>
/// Base interface for all domain events
/// Used for triggering side effects across aggregates
/// </summary>
public interface IDomainEvent
{
  DateTime OccurredOn { get; }
}

/// <summary>
/// Base implementation of domain event
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
  public DateTime OccurredOn { get; } = DateTime.UtcNow;
}