using FairWorkly.Domain.Common.Enums;
using FluentValidation;

namespace FairWorkly.Application.Payroll.Features.ValidatePayroll;

public class ValidatePayrollValidator : AbstractValidator<ValidatePayrollCommand>
{
    private static readonly string[] ValidAwardTypes = Enum.GetNames<AwardType>();
    private static readonly string[] ValidStates = Enum.GetNames<AustralianState>();

    public ValidatePayrollValidator()
    {
        RuleFor(x => x.FileStream)
            .NotNull()
            .WithMessage("File is required")
            .OverridePropertyName("file");

        RuleFor(x => x.FileName)
            .Must(name => name.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            .When(x => x.FileStream != null)
            .WithMessage("File must be a CSV file (.csv)")
            .OverridePropertyName("file");

        RuleFor(x => x.FileSize)
            .LessThanOrEqualTo(2 * 1024 * 1024L)
            .When(x => x.FileStream != null)
            .WithMessage("File size must not exceed 2MB")
            .OverridePropertyName("file");

        RuleFor(x => x.AwardType)
            .NotEmpty()
            .WithMessage("Award type is required")
            .OverridePropertyName("awardType");

        RuleFor(x => x.AwardType)
            .Must(value => ValidAwardTypes.Contains(value))
            .When(x => !string.IsNullOrEmpty(x.AwardType))
            .WithMessage($"Award type must be one of: {string.Join(", ", ValidAwardTypes)}")
            .OverridePropertyName("awardType");

        // MVP: only General Retail Industry Award is supported
        RuleFor(x => x.AwardType)
            .Must(value => value == nameof(AwardType.GeneralRetailIndustryAward2020))
            .When(x => !string.IsNullOrEmpty(x.AwardType) && ValidAwardTypes.Contains(x.AwardType))
            .WithMessage("Only General Retail Industry Award is currently supported")
            .OverridePropertyName("awardType");

        RuleFor(x => x.State)
            .Must(value => ValidStates.Contains(value))
            .When(x => !string.IsNullOrEmpty(x.State))
            .WithMessage($"State must be one of: {string.Join(", ", ValidStates)}")
            .OverridePropertyName("state");
    }
}
