namespace FairWorkly.Domain.Common.Enums;

/// <summary>
/// Australian states and territories
/// MVP: Only VIC supported, others for future expansion
/// </summary>
public enum AustralianState
{
    /// <summary>
    /// Victoria (Melbourne)
    /// MVP: Supported
    /// </summary>
    VIC = 1,

    /// <summary>
    /// New South Wales (Sydney)
    /// Post-MVP
    /// </summary>
    NSW = 2,

    /// <summary>
    /// Queensland (Brisbane)
    /// Post-MVP
    /// </summary>
    QLD = 3,

    /// <summary>
    /// South Australia (Adelaide)
    /// Post-MVP
    /// </summary>
    SA = 4,

    /// <summary>
    /// Western Australia (Perth)
    /// Post-MVP
    /// </summary>
    WA = 5,

    /// <summary>
    /// Tasmania (Hobart)
    /// Post-MVP
    /// </summary>
    TAS = 6,

    /// <summary>
    /// Australian Capital Territory (Canberra)
    /// Post-MVP
    /// </summary>
    ACT = 7,

    /// <summary>
    /// Northern Territory (Darwin)
    /// Post-MVP
    /// </summary>
    NT = 8,
}
