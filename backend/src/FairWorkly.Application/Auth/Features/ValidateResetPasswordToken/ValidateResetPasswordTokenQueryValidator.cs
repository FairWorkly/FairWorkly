using FluentValidation;

namespace FairWorkly.Application.Auth.Features.ValidateResetPasswordToken;

public class ValidateResetPasswordTokenQueryValidator
    : AbstractValidator<ValidateResetPasswordTokenQuery>
{
    public ValidateResetPasswordTokenQueryValidator()
    {
        RuleFor(x => x.Token).NotEmpty().WithMessage("Password reset token is required.");
    }
}
