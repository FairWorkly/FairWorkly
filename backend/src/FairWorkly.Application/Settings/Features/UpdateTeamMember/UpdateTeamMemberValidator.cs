using FluentValidation;

namespace FairWorkly.Application.Settings.Features.UpdateTeamMember;

public class UpdateTeamMemberValidator : AbstractValidator<UpdateTeamMemberCommand>
{
    public UpdateTeamMemberValidator()
    {
        RuleFor(x => x.TargetUserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Role)
            .Must(role => string.IsNullOrEmpty(role) || role == "Admin" || role == "Manager")
            .WithMessage("Role must be Admin or Manager");

        // At least one field must be provided
        RuleFor(x => x)
            .Must(x => x.Role != null || x.IsActive != null)
            .WithMessage("At least one field (role or isActive) must be provided");
    }
}
