using FluentValidation;

namespace FairWorkly.Application.Auth.Features.Logout;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x)
            .Must(cmd => cmd.UserId.HasValue || !string.IsNullOrWhiteSpace(cmd.RefreshTokenPlain))
            .WithMessage("Either UserId or RefreshTokenPlain must be provided.");

        When(
            x => x.UserId.HasValue,
            () =>
            {
                RuleFor(x => x.UserId)
                    .NotEqual(Guid.Empty)
                    .WithMessage("UserId must be a valid GUID.");
            }
        );
    }
}
