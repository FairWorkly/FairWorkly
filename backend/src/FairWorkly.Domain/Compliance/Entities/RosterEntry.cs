using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Employees.Entities;

namespace FairWorkly.Domain.Compliance.Entities;

/// <summary>
/// Individual roster entry (one shift for one employee)
/// </summary>
public class RosterEntry : BaseEntity
{
  public Guid RosterId { get; set; }
  public virtual Roster Roster { get; set; } = null!;

  public Guid EmployeeId { get; set; }
  public virtual Employee Employee { get; set; } = null!;

  public DateTime ShiftDate { get; set; }
  public TimeSpan ShiftStart { get; set; }
  public TimeSpan ShiftEnd { get; set; }
  public decimal TotalHours { get; set; }

  // Shift characteristics
  public bool IsWeekend { get; set; }
  public bool IsPublicHoliday { get; set; }
  public bool IsOvernight { get; set; } // Shift crosses midnight

  // Break time (unpaid)
  public decimal BreakMinutes { get; set; }
}