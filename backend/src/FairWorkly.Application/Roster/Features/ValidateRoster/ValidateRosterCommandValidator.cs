using FluentValidation;

namespace FairWorkly.Application.Roster.Features.ValidateRoster;

public class ValidateRosterCommandValidator : AbstractValidator<ValidateRosterCommand>
{
    public ValidateRosterCommandValidator()
    {
        RuleFor(x => x.RosterId).NotEmpty().WithMessage("RosterId is required.");
        RuleFor(x => x.OrganizationId).NotEmpty().WithMessage("OrganizationId is required.");
    }
}

