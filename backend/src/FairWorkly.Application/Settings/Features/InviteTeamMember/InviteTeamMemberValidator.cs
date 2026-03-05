using System.ComponentModel.DataAnnotations;
using FairWorkly.Domain.Auth.Enums;
using FluentValidation;

namespace FairWorkly.Application.Settings.Features.InviteTeamMember;

public class InviteTeamMemberValidator : AbstractValidator<InviteTeamMemberCommand>
{
    public InviteTeamMemberValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty().WithMessage("Organization ID is required.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .Must(email =>
                !string.IsNullOrWhiteSpace(email)
                && new EmailAddressAttribute().IsValid(email.Trim())
            )
            .WithMessage("A valid email is required.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MaximumLength(100)
            .WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MaximumLength(100)
            .WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Role is required.")
            .Must(role =>
                Enum.TryParse<UserRole>(role, ignoreCase: true, out var parsed)
                && parsed != UserRole.Unknown
            )
            .WithMessage("Role must be a valid role (e.g. Admin, Manager).");
    }
}
