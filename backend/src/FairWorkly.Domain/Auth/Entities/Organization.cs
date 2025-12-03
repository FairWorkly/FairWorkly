using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Employees.Entities;

namespace FairWorkly.Domain.Auth.Entities;

/// <summary>
/// Organization entity for multi-tenancy
/// Represents SME businesses using FairWorkly
/// </summary>
public class Organization : AuditableEntity
{
    // Basic Information
    public string Name { get; set; } = string.Empty;
    public string? ABN { get; set; } // Australian Business Number
    public string? Industry { get; set; }

    // Address (Australian format)
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? Suburb { get; set; }
    public string? State { get; set; } // NSW, VIC, QLD, etc.
    public string? Postcode { get; set; } // 4 digits

    // Subscription - Based on employee count
    public SubscriptionTier SubscriptionTier { get; set; }
    public DateTime SubscriptionStartDate { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Business Settings
    public int MaxEmployees { get; set; } = 50; // Default: Tier1
    public string? DefaultAwardCode { get; set; }
    public decimal SuperGuaranteeRate { get; set; } = 0.12m; // 12% for 2025/26

    // Navigation properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
