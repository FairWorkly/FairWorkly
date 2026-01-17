using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Payroll.Entities;

namespace FairWorkly.Application.Payroll.Services.ComplianceEngine;

/// <summary>
/// Validates that superannuation is paid at the required 12% rate
/// Applies to all employees with earnings
/// </summary>
public class SuperannuationRule : IComplianceRule
{
    public string RuleName => "Superannuation Check";

    public List<PayrollIssue> Evaluate(Payslip payslip, Guid validationId)
    {
        var issues = new List<PayrollIssue>();

        // Calculate total work hours
        var totalWorkHours = payslip.OrdinaryHours
            + payslip.SaturdayHours
            + payslip.SundayHours
            + payslip.PublicHolidayHours;

        // Check if there's no gross pay
        if (payslip.GrossPay <= 0)
        {
            // If there are work hours but no gross pay, that's a data issue
            if (totalWorkHours > 0)
            {
                issues.Add(new PayrollIssue
                {
                    OrganizationId = payslip.OrganizationId,
                    PayrollValidationId = validationId,
                    PayslipId = payslip.Id,
                    EmployeeId = payslip.EmployeeId,
                    CategoryType = IssueCategory.Superannuation,
                    Severity = IssueSeverity.Warning,
                    WarningMessage = "Missing Gross Pay Data: Cannot verify superannuation compliance",
                    ExpectedValue = 0,
                    ActualValue = 0,
                    AffectedUnits = totalWorkHours,
                    UnitType = "Hour",
                    ContextLabel = "Data Issue",
                    ImpactAmount = 0
                });
            }

            // Either way, can't check super without gross pay
            return issues;
        }

        // Calculate expected superannuation (12% of gross pay)
        var expectedSuper = payslip.GrossPay * RateTableProvider.SuperannuationRate;

        // Check if superannuation is underpaid
        if (payslip.Superannuation < expectedSuper - RateTableProvider.PayTolerance)
        {
            var impactAmount = expectedSuper - payslip.Superannuation;

            issues.Add(new PayrollIssue
            {
                OrganizationId = payslip.OrganizationId,
                PayrollValidationId = validationId,
                PayslipId = payslip.Id,
                EmployeeId = payslip.EmployeeId,
                CategoryType = IssueCategory.Superannuation,
                Severity = IssueSeverity.Error,
                ExpectedValue = expectedSuper,
                ActualValue = payslip.Superannuation,
                AffectedUnits = payslip.GrossPay,
                UnitType = "Currency",
                ContextLabel = $"12% of ${payslip.GrossPay:F2}",
                ImpactAmount = impactAmount
            });
        }

        return issues;
    }
}
