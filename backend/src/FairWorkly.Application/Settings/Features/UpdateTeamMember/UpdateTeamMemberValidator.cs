using FluentValidation;

namespace FairWorkly.Application.Settings.Features.UpdateTeamMember;

/// <summary>
/// Validates UpdateTeamMemberCommand before it reaches the handler.
/// Runs automatically via MediatR ValidationBehavior pipeline.
/// </summary>
public class UpdateTeamMemberValidator : AbstractValidator<UpdateTeamMemberCommand>
{
    private static readonly string[] ValidRoles = { "Admin", "Manager" };

    public UpdateTeamMemberValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.RequestingUserOrganizationId)
            .NotEmpty()
            .WithMessage("Organization context is required");

        // Role validation (only if provided)
        When(x => x.Role != null, () =>
        {
            RuleFor(x => x.Role)
                .Must(role => ValidRoles.Contains(role))
                .WithMessage("Role must be 'Admin' or 'Manager'");
        });

        // At least one field must be provided for update
        RuleFor(x => x)
            .Must(x => x.Role != null || x.IsActive != null)
            .WithMessage("At least one field (role or isActive) must be provided");
    }
}
