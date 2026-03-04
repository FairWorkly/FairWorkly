namespace FairWorkly.Application.Settings.Features.OrganizationProfile.UpdateOrganizationProfile;

public class UpdateOrganizationProfileRequest
{
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Australian Business Number (must be 11 digits)
    /// </summary>
    public string ABN { get; set; } = string.Empty;

    public string IndustryType { get; set; } = string.Empty;

    public string ContactEmail { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string AddressLine1 { get; set; } = string.Empty;

    public string? AddressLine2 { get; set; }

    public string Suburb { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Australian postcode (must be 4 digits)
    /// </summary>
    public string Postcode { get; set; } = string.Empty;

    public string? LogoUrl { get; set; }
}
