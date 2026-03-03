using FairWorkly.Domain.Auth.Enums;
using FluentValidation;

namespace FairWorkly.Application.Settings.Features.UpdateTeamMember;

public class UpdateTeamMemberValidator : AbstractValidator<UpdateTeamMemberCommand>
{
    public UpdateTeamMemberValidator()
    {
        RuleFor(x => x.TargetUserId).NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Role)
            .Must(role =>
                string.IsNullOrWhiteSpace(role)
                || (
                    Enum.TryParse<UserRole>(role, ignoreCase: true, out var parsed)
                    && parsed != UserRole.Unknown
                )
            )
            .WithMessage("Role must be a valid role (e.g. Admin, Manager)");

        // At least one field must be provided
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Role) || x.IsActive != null)
            .WithMessage("At least one field (role or isActive) must be provided");
    }
}
