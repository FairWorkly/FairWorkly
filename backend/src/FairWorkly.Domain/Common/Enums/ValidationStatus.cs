namespace FairWorkly.Domain.Common.Enums;

/// <summary>
/// Status of compliance validation
/// Used by: Payroll Agent, Compliance Agent
/// </summary>
public enum ValidationStatus
{
  /// <summary>
  /// Validation not yet started
  /// </summary>
  Pending = 1,

  /// <summary>
  /// Validation in progress
  /// </summary>
  InProgress = 2,

  /// <summary>
  /// Validation completed - all checks passed
  /// </summary>
  Passed = 3,

  /// <summary>
  /// Validation completed - issues found
  /// </summary>
  Failed = 4
}