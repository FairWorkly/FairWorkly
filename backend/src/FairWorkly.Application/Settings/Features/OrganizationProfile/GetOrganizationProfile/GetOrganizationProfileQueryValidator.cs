using FluentValidation;

namespace FairWorkly.Application.Settings.Features.OrganizationProfile.GetOrganizationProfile;

public class GetOrganizationProfileQueryValidator : AbstractValidator<GetOrganizationProfileQuery>
{
    public GetOrganizationProfileQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Organization ID is required")
            .Must(BeValidGuid)
            .WithMessage("Organization ID must be a valid GUID");
    }

    private bool BeValidGuid(string? orgId)
    {
        return Guid.TryParse(orgId, out _);
    }
}
