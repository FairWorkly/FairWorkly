using FluentValidation;

namespace FairWorkly.Application.Auth.Features.Refresh;

public class RefreshCommandValidator : AbstractValidator<RefreshCommand>
{
    public RefreshCommandValidator()
    {
        RuleFor(x => x.RefreshTokenPlain)
            .NotEmpty()
            .WithMessage("Refresh token is required.");
    }
}
