using System.ComponentModel.DataAnnotations;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Domain.Auth.Entities;

/// <summary>
/// Many-to-many relationship between Organization and Awards
/// Represents the "Active Awards" section in Settings
/// </summary>
public class OrganizationAward : BaseEntity
{
  /// <summary>
  /// Organization ID
  /// </summary>
  [Required]
  public Guid OrganizationId { get; set; }

  public virtual Organization Organization { get; set; } = null!;

  /// <summary>
  /// Which Award type applies to this organization
  /// </summary>
  [Required]
  public AwardType AwardType { get; set; }

  /// <summary>
  /// When this Award was added to the organization
  /// </summary>
  public DateTime AddedAt { get; set; }

  /// <summary>
  /// Is this the default/primary Award?
  /// </summary>
  public bool IsPrimary { get; set; } = false;

  /// <summary>
  /// How many employees currently use this Award
  /// Cached for performance, updated when employees are added/removed
  /// </summary>
  public int EmployeeCount { get; set; } = 0;
}