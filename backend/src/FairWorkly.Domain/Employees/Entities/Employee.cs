using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Domain.Employees.Entities;

/// <summary>
/// Employee entity - shared by all agents
/// Central to Compliance, Payroll, Documents, and Employee Help
/// </summary>
public class Employee : AuditableEntity
{
    // Basic Info
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }

    // Employment Details
    public string EmployeeNumber { get; set; } = string.Empty;
    public EmploymentType EmploymentType { get; set; }
    public AwardType? ApplicableAward { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Position
    public string JobTitle { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Location { get; set; } // Store/office location

    // Compensation
    public decimal? AnnualSalary { get; set; } // For full-time/part-time
    public decimal? HourlyRate { get; set; } // For casual/hourly

    // Leave Balances (simplified for MVP)
    public decimal AnnualLeaveBalance { get; set; }
    public decimal PersonalLeaveBalance { get; set; }

    // Multi-tenancy
    public Guid OrganizationId { get; set; }
    public virtual Organization Organization { get; set; } = null!;

    // Link to User account (A employee has portal access)
    public Guid? UserId { get; set; }
    public virtual User? User { get; set; }

    // ⚠️ NOTE: Navigation properties for RosterEntries and PayrollEntries
    // will be added later when those entities are created

    // Computed property
    public string FullName => $"{FirstName} {LastName}";
    public int Age => DateTime.Today.Year - DateOfBirth.Year;
}
