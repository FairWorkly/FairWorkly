using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace FairWorkly.Application.Auth.Features.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .Must(email => !string.IsNullOrWhiteSpace(email) && new EmailAddressAttribute().IsValid(email.Trim()))
            .WithMessage("A valid email is required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters.");
    }
}
