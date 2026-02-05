using FairWorkly.Domain.Common.Enums;
using FluentValidation;

namespace FairWorkly.Application.Settings.Features.OrganizationProfile.UpdateOrganizationProfile;

public class UpdateOrganizationProfileCommandValidator
    : AbstractValidator<UpdateOrganizationProfileCommand>
{
    public UpdateOrganizationProfileCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty().WithMessage("Organization ID is required");

        RuleFor(x => x.Request)
            .NotNull()
            .WithMessage("Request data is required")
            .SetValidator(new UpdateOrganizationProfileRequestValidator());
    }
}

public class UpdateOrganizationProfileRequestValidator
    : AbstractValidator<UpdateOrganizationProfileRequest>
{
    public UpdateOrganizationProfileRequestValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .WithMessage("Company name is required")
            .MaximumLength(200)
            .WithMessage("Company name must not exceed 200 characters");

        RuleFor(x => x.ABN)
            .NotEmpty()
            .WithMessage("ABN is required")
            .Matches(@"^\d{11}$")
            .WithMessage("ABN must be exactly 11 digits");

        RuleFor(x => x.IndustryType)
            .NotEmpty()
            .WithMessage("Industry type is required")
            .MaximumLength(100)
            .WithMessage("Industry type must not exceed 100 characters");

        RuleFor(x => x.ContactEmail)
            .NotEmpty()
            .WithMessage("Contact email is required")
            .EmailAddress()
            .WithMessage("Contact email must be a valid email address")
            .MaximumLength(255)
            .WithMessage("Contact email must not exceed 255 characters");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .WithMessage("Phone number must not exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.AddressLine1)
            .NotEmpty()
            .WithMessage("Address line 1 is required")
            .MaximumLength(200)
            .WithMessage("Address line 1 must not exceed 200 characters");

        RuleFor(x => x.AddressLine2)
            .MaximumLength(200)
            .WithMessage("Address line 2 must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.AddressLine2));

        RuleFor(x => x.Suburb)
            .NotEmpty()
            .WithMessage("Suburb is required")
            .MaximumLength(100)
            .WithMessage("Suburb must not exceed 100 characters");

        RuleFor(x => x.State)
            .NotEmpty()
            .WithMessage("State is required")
            .Must(BeValidAustralianState)
            .WithMessage("State must be a valid Australian state/territory code")
            .Must(state => state.Equals("VIC", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Currently only VIC (Victoria) is supported in MVP");

        RuleFor(x => x.Postcode)
            .NotEmpty()
            .WithMessage("Postcode is required")
            .Matches(@"^\d{4}$")
            .WithMessage("Postcode must be exactly 4 digits");

        RuleFor(x => x.LogoUrl)
            .MaximumLength(500)
            .WithMessage("Logo URL must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.LogoUrl));
    }

    private bool BeValidAustralianState(string state)
    {
        if (string.IsNullOrWhiteSpace(state))
            return false;
        return Enum.IsDefined(typeof(AustralianState), state.ToUpper());
    }
}
