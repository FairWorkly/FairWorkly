namespace FairWorkly.Domain.Employees.Entities;

public class Employee
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; } // Multi-tenant Isolation Field
    public string? ExternalRef { get; set; } // ID from external system (e.g. Xero)

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }

    public string? EmploymentType { get; set; } // e.g. FullTime, Casual
    public string Status { get; set; } = "ACTIVE";

    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public string? AwardCode { get; set; }
    public decimal? BaseHourlyRate { get; set; }
    public string? PositionTitle { get; set; }

    // Audit Fields
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}