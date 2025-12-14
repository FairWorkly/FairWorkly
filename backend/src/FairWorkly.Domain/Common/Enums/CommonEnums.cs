namespace FairWorkly.Domain.Common.Enums;

/// <summary>
/// Employment types as per Australian Fair Work Act
/// </summary>
public enum EmploymentType
{
    /// <summary>
    /// Full-time permanent employment
    /// Entitled to: Annual leave, Personal leave, Long service leave
    /// </summary>
    FullTime = 1,
    /// <summary>
    /// Part-time permanent employment
    /// Entitled to: Pro-rata annual leave, personal leave, long service leave
    /// </summary>
    PartTime = 2,
    /// <summary>
    /// Casual employment with 25% loading
    /// Not entitled to: Annual leave, personal leave
    /// Can refuse shifts with reasonable notice
    /// </summary>
    Casual = 3,
    /// <summary>
    /// Fixed-term contract employment
    /// Entitlements depend on contract terms
    /// </summary>
    FixedTerm = 4,
}

/// <summary>
/// Issue severity levels for compliance checks
/// Used in PayrollIssue and RosterIssue
/// </summary>
public enum IssueSeverity
{
    /// <summary>
    /// Informational only - no action required
    /// Example: "Employee worked 38 hours (full-time standard)"
    /// </summary>
    Info = 1,

    /// <summary>
    /// Warning - should fix but not critical
    /// Example: "Employee worked 39 hours (1 hour over standard)"
    /// </summary>
    Warning = 2,

    /// <summary>
    /// Error - must fix to ensure compliance
    /// Example: "Saturday penalty rate not applied"
    /// </summary>
    Error = 3,

    /// <summary>
    /// Critical - legal violation, immediate action required
    /// Example: "Employee paid below minimum wage"
    /// </summary>
    Critical = 4
}

/// <summary>
/// Document types for contract and letter generation
/// </summary>
public enum DocumentType
{
    EmploymentContract = 1,              // Priority 1
    FairWorkInformationStatement = 2,    // Priority 2
    PositionDescription = 3,             // Priority 3

}

/// <summary>
/// Validation status for Payroll and Roster Compliance checks
/// </summary>
public enum ValidationStatus
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
    /// <summary>
    /// General Retail Industry Award 2020 (MA000004)
    /// Covers: Retail stores, supermarkets, department stores
    /// </summary>
    Retail = 1,

    /// <summary>
    /// Hospitality Industry (General) Award 2020 (MA000009)
    /// Covers: Restaurants, cafes, hotels, catering
    /// </summary>
    Hospitality = 2,

    /// <summary>
    /// Clerks - Private Sector Award 2020 (MA000002)
    /// Covers: Office administration, clerical work
    /// </summary>
    Clerks = 3

    // Future: Add more awards as needed
}

// <summary>
/// Australian states and territories
/// 
/// MVP Support: Victoria (VIC) only
/// Post-MVP Roadmap: NSW → QLD → WA → SA → TAS → ACT → NT
/// 
/// Note: Fair Work Awards are national (not state-specific)
/// State mainly affects Public Holidays and Long Service Leave rules
/// </summary>
public enum AustralianState
{
    /// <summary>
    /// Victoria
    /// Capital: Melbourne | Population: 6.7M | Market Share: 27%
    /// Status: ✅ MVP Supported
    /// </summary>
    VIC = 1,

    /// <summary>
    /// New South Wales
    /// Capital: Sydney | Population: 8.2M | Market Share: 32%
    /// Status: 📝 Post-MVP Phase 1
    /// </summary>
    NSW = 2,

    /// <summary>
    /// Queensland
    /// Capital: Brisbane | Population: 5.4M | Market Share: 21%
    /// Status: 📝 Post-MVP Phase 2
    /// </summary>
    QLD = 3,

    /// <summary>
    /// South Australia
    /// Capital: Adelaide | Population: 1.8M | Market Share: 7%
    /// Status: 📝 Post-MVP Phase 3
    /// </summary>
    SA = 4,

    /// <summary>
    /// Western Australia
    /// Capital: Perth | Population: 2.8M | Market Share: 11%
    /// Status: 📝 Post-MVP Phase 3
    /// </summary>
    WA = 5,

    /// <summary>
    /// Tasmania
    /// Capital: Hobart | Population: 0.57M | Market Share: 2%
    /// Status: 📝 Post-MVP Phase 4
    /// </summary>
    TAS = 6,

    /// <summary>
    /// Northern Territory
    /// Capital: Darwin | Population: 0.25M | Market Share: <1%
    /// Status: 📝 Post-MVP Phase 4
    /// </summary>
    NT = 7,

    /// <summary>
    /// Australian Capital Territory
    /// Capital: Canberra | Population: 0.46M | Market Share: 1%
    /// Status: 📝 Post-MVP Phase 4
    /// </summary>
    ACT = 8
}
