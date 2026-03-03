using FluentValidation;

namespace FairWorkly.Application.Payroll.Features.ExplainIssue;

public class ExplainPayrollIssueValidator : AbstractValidator<ExplainPayrollIssueCommand>
{
    private static readonly string[] ValidCategories =
    [
        "BaseRate",
        "PenaltyRate",
        "CasualLoading",
        "Superannuation",
    ];

    public ExplainPayrollIssueValidator()
    {
        RuleFor(x => x.IssueId).NotEmpty().WithMessage("IssueId is required.");

        RuleFor(x => x.CategoryType)
            .NotEmpty()
            .WithMessage("CategoryType is required.")
            .Must(c => ValidCategories.Contains(c))
            .WithMessage(
                "CategoryType must be one of: BaseRate, PenaltyRate, CasualLoading, Superannuation."
            );

        RuleFor(x => x.EmployeeName).NotEmpty().WithMessage("EmployeeName is required.");

        RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("EmployeeId is required.");
    }
}
