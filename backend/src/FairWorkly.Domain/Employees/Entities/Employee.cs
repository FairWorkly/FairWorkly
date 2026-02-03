using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Documents.Entities;
using FairWorkly.Domain.Payroll.Entities;
using FairWorkly.Domain.Roster.Entities;

namespace FairWorkly.Domain.Employees.Entities;

/// <summary>
/// Employee entity
/// Represents an employee in the organization
/// Used by: Payroll Agent, Compliance Agent, Document Agent
/// </summary>
public class Employee : AuditableEntity
{
    [Required]
    public Guid OrganizationId { get; set; }
    public virtual Organization Organization { get; set; } = null!;

    // Basic Info
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Full name (computed)
    /// </summary>
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Employee email address (optional)
    /// May be null for employees imported from payroll/roster files
    /// </summary>
    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; set; }

    [Phone]
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    // Employment Details
    [Required]
    [MaxLength(100)]
    public string JobTitle { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Department { get; set; }

    [Required]
    public EmploymentType EmploymentType { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Guaranteed hours per week (for Part-time only)
    /// </summary>
    public int? GuaranteedHours { get; set; }

    //Award & Pay
    /// <summary>
    /// Which Award applies to this employee
    /// </summary>
    [Required]
    public AwardType AwardType { get; set; }

    /// <summary>
    /// Award classification level (1-8)
    /// </summary>
    [Required]
    public int AwardLevelNumber { get; set; }

    /// <summary>
    /// Company-defined identifier, different from database Id
    /// Example: "EMP001", "H-2024-042", "MEL-BAR-001"
    /// Used for: CSV matching, payslip identification, display in UI
    /// </summary>
    [MaxLength(50)]
    public string? EmployeeNumber { get; set; }

    //Tax & Super
    [MaxLength(20)]
    public string? TaxFileNumber { get; set; }

    [MaxLength(100)]
    public string? SuperannuationFund { get; set; }

    [MaxLength(50)]
    public string? SuperannuationMemberNumber { get; set; }

    //Navigation Properties

    /// <summary>
    /// All payslips for this employee
    /// Used by: Payroll Agent to validate pay history
    /// </summary>
    public virtual ICollection<Payslip> Payslips { get; set; } = new List<Payslip>();

    /// <summary>
    /// All shifts assigned to this employee
    /// Used by: Compliance Agent to check roster compliance
    /// </summary>
    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();

    /// <summary>
    /// All documents generated for this employee
    /// Used by: Document Agent (contracts, FWIS, position descriptions)
    /// </summary>
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
}
