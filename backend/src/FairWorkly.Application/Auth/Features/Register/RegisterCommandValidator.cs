using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using FairWorkly.Domain.Common.Enums;
using FluentValidation;

namespace FairWorkly.Application.Auth.Features.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .WithMessage("Company name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Abn)
            .NotEmpty()
            .WithMessage("ABN is required.")
            .Matches(@"^\d{11}$")
            .WithMessage("ABN must be 11 digits.");

        RuleFor(x => x.IndustryType)
            .NotEmpty()
            .WithMessage("Industry type is required.")
            .MaximumLength(100);

        RuleFor(x => x.AddressLine1)
            .NotEmpty()
            .WithMessage("Address line 1 is required.")
            .MaximumLength(200);

        RuleFor(x => x.Suburb).NotEmpty().WithMessage("Suburb is required.").MaximumLength(100);

        RuleFor(x => x.State)
            .NotEmpty()
            .WithMessage("State is required.")
            .Must(BeValidState)
            .WithMessage("State must be a valid Australian state.");

        RuleFor(x => x.Postcode)
            .NotEmpty()
            .WithMessage("Postcode is required.")
            .Matches(@"^\d{4}$")
            .WithMessage("Postcode must be 4 digits.");

        RuleFor(x => x.ContactEmail)
            .NotEmpty()
            .WithMessage("Contact email is required.")
            .Must(email =>
                !string.IsNullOrWhiteSpace(email)
                && new EmailAddressAttribute().IsValid(email.Trim())
            )
            .WithMessage("A valid contact email is required.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .Must(email =>
                !string.IsNullOrWhiteSpace(email)
                && new EmailAddressAttribute().IsValid(email.Trim())
            )
            .WithMessage("A valid email is required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters.")
            .Must(HasLetterAndNumber)
            .WithMessage("Password must include both letters and numbers.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MaximumLength(100);
    }

    private static bool HasLetterAndNumber(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        return Regex.IsMatch(password, "[A-Za-z]") && Regex.IsMatch(password, "[0-9]");
    }

    private static bool BeValidState(string state)
    {
        if (string.IsNullOrWhiteSpace(state))
        {
            return false;
        }

        return Enum.TryParse<AustralianState>(state, true, out _);
    }
}
