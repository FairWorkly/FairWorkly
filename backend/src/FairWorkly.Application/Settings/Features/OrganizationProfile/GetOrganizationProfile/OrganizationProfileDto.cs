namespace FairWorkly.Application.Settings.Features.OrganizationProfile.GetOrganizationProfile;

public class OrganizationProfileDto
{
    public string? LogoUrl { get; set; }
    public required string CompanyName { get; set; }

    /// <summary>
    /// Australian Business Number (11 digits)
    /// </summary>
    public required string ABN { get; set; }

    public required string IndustryType { get; set; }

    public required string AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public required string ContactEmail { get; set; }

    public required string Suburb { get; set; }

    /// <summary>
    /// Australian state/territory code (e.g., "VIC", "NSW")
    /// </summary>
    public required string State { get; set; }

    /// <summary>
    /// Australian postcode (4 digits)
    /// </summary>
    public required string Postcode { get; set; }
    public string? PhoneNumber { get; set; }
}
