namespace FairWorkly.Domain.Common.Enums;

/// <summary>
/// Employment types as per Australian Fair Work Act
/// </summary>
public enum EmploymentType
{
  /// <summary>
  /// Full-time permanent employment
  /// 38 hours per week
  /// Entitled to annual leave, sick leave
  /// Notice period: 4 weeks
  /// </summary>
  FullTime = 1,
  /// <summary>
  /// Part-time permanent employment
  /// Guaranteed hours per week (less than 38)
  /// Pro-rata annual leave, sick leave
  /// Notice period: As per Award
  /// </summary>
  PartTime = 2,
  /// <summary>
  /// No guaranteed hours
  /// Casual employment with 25% loading
  ///No paid leave
  /// Can refuse shifts with reasonable notice
  /// Notice period: 1 hour
  /// </summary>
  Casual = 3,
  /// <summary>
  /// Fixed-term contract employment
  /// - Similar to full-time but with defined end date
  /// - Annual leave, sick leave (pro-rata if contract < 12 months)
  /// - Notice periods as per contract
  /// </summary>
  FixedTerm = 4,
}