namespace FairWorkly.Domain.Common.Enums;

/// <summary>
/// Severity level of compliance issues
/// Used by: PayrollIssue, RosterIssue
///
/// PERSISTENCE CONTRACT:
/// - This enum is persisted to the database as a string (enum name) via EF Core HasConversion&lt;string&gt;().
/// - Once a name is released, DO NOT rename or delete members; only add new members.
/// - If a rename is unavoidable, a data migration must update existing stored strings accordingly.
/// </summary>
public enum IssueSeverity
{
    /// <summary>
    /// Informational - no action required
    /// </summary>
    Info = 1,

    /// <summary>
    /// Warning - should be reviewed but not critical
    /// Example: Weekly hours slightly over 38
    /// </summary>
    Warning = 2,

    /// <summary>
    /// Error - must be fixed before processing
    /// Example: Incorrect penalty rate calculation
    /// </summary>
    Error = 3,

    /// <summary>
    /// Critical - severe Fair Work violation
    /// Example: Paying below minimum wage
    /// </summary>
    Critical = 4,
}
