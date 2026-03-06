using FluentValidation;

namespace FairWorkly.Application.Settings.Features.ResendInvitation;

public class ResendInvitationValidator : AbstractValidator<ResendInvitationCommand>
{
    public ResendInvitationValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty().WithMessage("Organization ID is required.");
        RuleFor(x => x.TargetUserId).NotEmpty().WithMessage("User ID is required.");
    }
}
