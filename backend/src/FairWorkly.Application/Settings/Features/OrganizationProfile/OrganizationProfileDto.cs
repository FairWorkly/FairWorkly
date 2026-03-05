using FairWorkly.Domain.Auth.Entities;

namespace FairWorkly.Application.Settings.Features.OrganizationProfile;

public class OrganizationProfileDto
{
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Australian Business Number (11 digits)
    /// </summary>
    public string ABN { get; set; } = string.Empty;

    public string IndustryType { get; set; } = string.Empty;

    public string ContactEmail { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string AddressLine1 { get; set; } = string.Empty;

    public string? AddressLine2 { get; set; }

    public string Suburb { get; set; } = string.Empty;

    /// <summary>
    /// Australian state/territory code (e.g., "VIC", "NSW")
    /// </summary>
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Australian postcode (4 digits)
    /// </summary>
    public string Postcode { get; set; } = string.Empty;

    public string? LogoUrl { get; set; }

    public static OrganizationProfileDto FromEntity(Organization organization) =>
        new()
        {
            CompanyName = organization.CompanyName,
            ABN = organization.ABN,
            IndustryType = organization.IndustryType,
            ContactEmail = organization.ContactEmail,
            PhoneNumber = organization.PhoneNumber,
            AddressLine1 = organization.AddressLine1,
            AddressLine2 = organization.AddressLine2,
            Suburb = organization.Suburb,
            State = organization.State.ToString(),
            Postcode = organization.Postcode,
            LogoUrl = organization.LogoUrl,
        };
}
