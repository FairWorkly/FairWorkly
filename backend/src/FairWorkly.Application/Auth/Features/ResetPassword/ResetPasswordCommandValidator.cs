using System.Text.RegularExpressions;
using FluentValidation;

namespace FairWorkly.Application.Auth.Features.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty().WithMessage("Password reset token is required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters.")
            .MaximumLength(128)
            .WithMessage("Password must not exceed 128 characters.")
            .Must(HasLetterAndNumber)
            .WithMessage("Password must include both letters and numbers.");
    }

    private static bool HasLetterAndNumber(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        return Regex.IsMatch(password, "[A-Za-z]") && Regex.IsMatch(password, "[0-9]");
    }
}
