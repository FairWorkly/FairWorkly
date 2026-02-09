using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Payroll.Enums;
using FairWorkly.Domain.Payroll.Entities;

namespace FairWorkly.Domain.Payroll.ComplianceEngine;

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
        var totalWorkHours =
            payslip.OrdinaryHours
            + payslip.SaturdayHours
            + payslip.SundayHours
            + payslip.PublicHolidayHours
            + (payslip.OvertimeHours ?? 0m);

        // Scenario A: GrossPay is negative (correction/reversal entry)
        if (payslip.GrossPay < 0)
        {
            issues.Add(
                new PayrollIssue
                {
                    OrganizationId = payslip.OrganizationId,
                    PayrollValidationId = validationId,
                    PayslipId = payslip.Id,
                    EmployeeId = payslip.EmployeeId,
                    CategoryType = IssueCategory.Superannuation,
                    Severity = IssueSeverity.Warning,
                    WarningMessage =
                        $"Negative Gross Pay detected (-${Math.Abs(payslip.GrossPay):F2}). Possible correction/reversal entry. Skipping compliance check.",
                    ExpectedValue = 0,
                    ActualValue = 0,
                    AffectedUnits = 0,
                    UnitType = "Currency",
                    ContextLabel = "Data Issue",
                }
            );
            return issues;
        }

        // Scenario B: GrossPay is zero
        if (payslip.GrossPay == 0)
        {
            if (totalWorkHours > 0)
            {
                // Has work hours but zero pay - data anomaly
                issues.Add(
                    new PayrollIssue
                    {
                        OrganizationId = payslip.OrganizationId,
                        PayrollValidationId = validationId,
                        PayslipId = payslip.Id,
                        EmployeeId = payslip.EmployeeId,
                        CategoryType = IssueCategory.Superannuation,
                        Severity = IssueSeverity.Warning,
                        WarningMessage =
                            $"Zero Gross Pay but worked {totalWorkHours} hours. Please verify data.",
                        ExpectedValue = 0,
                        ActualValue = 0,
                        AffectedUnits = totalWorkHours,
                        UnitType = "Hour",
                        ContextLabel = "Data Issue",
                    }
                );
            }
            // No hours and no pay = unpaid leave, PASS
            return issues;
        }

        // Calculate expected superannuation (12% of gross pay)
        var expectedSuper = payslip.GrossPay * RateTableProvider.SuperannuationRate;

        // Check if superannuation is underpaid
        if (payslip.Superannuation < expectedSuper - RateTableProvider.PayTolerance)
        {
            issues.Add(
                new PayrollIssue
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
                }
            );
        }

        return issues;
    }
}
