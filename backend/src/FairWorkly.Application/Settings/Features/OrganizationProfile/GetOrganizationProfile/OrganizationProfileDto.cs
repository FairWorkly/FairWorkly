namespace FairWorkly.Application.Settings.Features.OrganizationProfile.GetOrganizationProfile;

public class OrganizationProfileDto
{
    public required string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Australian Business Number (11 digits)
    /// </summary>
    public required string ABN { get; set; } = string.Empty;

    public required string IndustryType { get; set; } = string.Empty;

    public required string ContactEmail { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public required string AddressLine1 { get; set; } = string.Empty;

    public string? AddressLine2 { get; set; }

    public required string Suburb { get; set; } = string.Empty;

    /// <summary>
    /// Australian state/territory code (e.g., "VIC", "NSW")
    /// </summary>
    public required string State { get; set; } = string.Empty;

    /// <summary>
    /// Australian postcode (4 digits)
    /// </summary>
    public required string Postcode { get; set; } = string.Empty;

    public string? LogoUrl { get; set; }
}
