namespace FairWorkly.Domain.Common.Enums;

/// <summary>
/// Category of compliance issue
/// </summary>
public enum IssueCategory
{
    /// <summary>
    /// Pre-validation failure (data missing or invalid)
    /// </summary>
    PreValidation = 0,

    /// <summary>
    /// Rule 1: Base rate underpayment
    /// </summary>
    BaseRate = 1,

    /// <summary>
    /// Rule 2: Penalty rate underpayment (Saturday/Sunday/Public Holiday)
    /// </summary>
    PenaltyRate = 2,

    /// <summary>
    /// Rule 3: Casual loading underpayment
    /// </summary>
    CasualLoading = 3,

    /// <summary>
    /// Rule 4: Superannuation underpayment
    /// </summary>
    Superannuation = 4,

    /// <summary>
    /// Rule 5: STP compliance (future feature)
    /// </summary>
    STPCompliance = 5,
}
