namespace FairWorkly.Domain.Common.Enums;

/// <summary>
/// Employment types as per Australian Fair Work Act
/// </summary>
public enum EmploymentType
{
    FullTime = 1,
    PartTime = 2,
    Casual = 3,
    FixedTerm = 4,
}

/// <summary>
/// Issue severity levels for compliance checks
/// </summary>
public enum IssueSeverity
{
    Low = 1, // Green - informational
    Medium = 2, // Yellow - potential issue
    High = 3, // Red - definite violation
    Critical = 4, // Immediate action required
}

/// <summary>
/// Document types for contract and letter generation
/// </summary>
public enum DocumentType
{
    OfferLetter = 1,
    EmploymentContract = 2,
    WarningLetter = 3,
    ShowCauseLetter = 4,
    TerminationLetter = 5,
    VariationAgreement = 6,
}

/// <summary>
/// Status of payroll run checks
/// </summary>
public enum PayrollCheckStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
}

/// <summary>
/// Compliance check status
/// </summary>
public enum ComplianceCheckStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
}

/// <summary>
/// Australian Modern Awards (MVP covers top 3)
/// </summary>
public enum AwardType
{
    None = 0,
    Retail = 1, // General Retail Industry Award
    Hospitality = 2, // Hospitality Industry (General) Award
    Clerks = 3, // Clerks - Private Sector Award
    // Future: Add more awards as needed
}
