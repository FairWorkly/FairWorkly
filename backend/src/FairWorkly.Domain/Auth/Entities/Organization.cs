using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Employees.Entities;

namespace FairWorkly.Domain.Auth.Entities;

/// <summary>
/// Organization entity - represents a business/company using FairWorkly
/// Multi-tenancy:Each Organization is a separate tenant with isolated data.
/// All other entities (Employee, Payslip, etc.) belong to an Organization.
/// </summary>
public class Organization : BaseEntity
{
    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [StringLength(11, MinimumLength = 11)]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "ABN must be 11 digits")]
    public string ABN { get; set; } = string.Empty;

    /// <summary>
    /// Primary industry type
    /// Used to pre-select common Award types
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string IndustryType { get; set; } = string.Empty;

    // Address (Australian format)
    [Required]
    [StringLength(200)]
    public string AddressLine1 { get; set; } = string.Empty;

    [StringLength(200)]
    public string? AddressLine2 { get; set; }

    [Required]
    [StringLength(100)]
    public string Suburb { get; set; } = string.Empty;

    [Required]
    public AustralianState State { get; set; } = AustralianState.VIC;

    [Required]
    [StringLength(4, MinimumLength = 4)]
    [RegularExpression(@"^\d{4}$")]
    public string Postcode { get; set; } = string.Empty;

    /// <summary>
    /// Full formatted address (computed property)
    /// Used for display purposes
    /// </summary>
    [NotMapped]
    public string FullAddress =>
        $"{AddressLine1}{(string.IsNullOrEmpty(AddressLine2) ? "" : ", " + AddressLine2)}, {Suburb} {State} {Postcode}";

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string ContactEmail { get; set; } = string.Empty;

    /// <summary>
    /// Business phone number
    /// Australian format with country code
    /// </summary>
    [Phone]
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Current subscription tier
    /// Determines MaxEmployees limit
    /// </summary>
    [Required]
    public SubscriptionTier SubscriptionTier { get; set; } = SubscriptionTier.Tier1;

    /// <summary>
    /// When subscription started
    /// Set during registration/payment
    /// </summary>
    [Required]
    public DateTime SubscriptionStartDate { get; set; }

    /// <summary>
    /// When current billing period ends
    /// </summary>
    public DateTime? SubscriptionEndDate { get; set; }

    /// <summary>
    /// Is subscription active
    /// </summary>
    [Required]
    public bool IsSubscriptionActive { get; set; } = true;

    /// <summary>
    /// Maximum employees based on tier
    /// Used for business logic validation
    /// </summary>
    [NotMapped]
    public int MaxEmployees => SubscriptionTier switch
    {
        SubscriptionTier.Tier1 => 50,
        SubscriptionTier.Tier2 => 100,
        SubscriptionTier.Tier3 => 150,
        _ => 50
    };

    /// <summary>
    /// Current employee count
    /// Used for limit checking
    /// </summary>
    public int CurrentEmployeeCount { get; set; } = 0;

    /// <summary>
    /// Users in this organization
    /// </summary>
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    /// <summary>
    /// Employees in this organization
    /// </summary>
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    /// <summary>
    /// Active Awards for this organization
    /// Shown in "Active Awards" section of Settings
    /// </summary>
    public virtual ICollection<OrganizationAward> OrganizationAwards { get; set; } = new List<OrganizationAward>();

}
