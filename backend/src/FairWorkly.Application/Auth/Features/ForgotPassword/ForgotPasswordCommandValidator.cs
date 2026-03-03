using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace FairWorkly.Application.Auth.Features.ForgotPassword;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .Must(email =>
                !string.IsNullOrWhiteSpace(email)
                && new EmailAddressAttribute().IsValid(email.Trim())
            )
            .WithMessage("A valid email is required.");
    }
}
