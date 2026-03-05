using FluentValidation;

namespace FairWorkly.Application.Settings.Features.CancelInvitation;

public class CancelInvitationValidator : AbstractValidator<CancelInvitationCommand>
{
    public CancelInvitationValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty().WithMessage("Organization ID is required.");
        RuleFor(x => x.TargetUserId).NotEmpty().WithMessage("User ID is required.");
    }
}
