using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Payroll.Entities;

namespace FairWorkly.Application.Payroll.Services.ComplianceEngine;

/// <summary>
/// Validates that Casual employees receive the 25% casual loading
/// Only applies to Casual employees - other employee types are skipped
/// Checks against Casual Rate (which includes the 25% loading)
/// </summary>
public class CasualLoadingRule : IComplianceRule
{
    public string RuleName => "Casual Loading Check";

    public List<PayrollIssue> Evaluate(Payslip payslip, Guid validationId)
    {
        var issues = new List<PayrollIssue>();

        // Only applies to Casual employees
        if (payslip.EmploymentType != EmploymentType.Casual)
        {
            return issues;
        }

        // Skip if no ordinary hours worked
        if (payslip.OrdinaryHours <= 0)
        {
            return issues;
        }

        // Get the minimum Casual rate (includes 25% loading)
        var level = RateTableProvider.ParseLevel(payslip.Classification);
        var casualRate = RateTableProvider.GetCasualRate(level);

        if (casualRate <= 0)
        {
            // Invalid level - should have been caught by pre-validation
            return issues;
        }

        // Calculate actual hourly rate from pay
        var actualRate = payslip.OrdinaryPay / payslip.OrdinaryHours;

        // Check 1: Is the actual paid rate below minimum Casual rate?
        if (actualRate < casualRate - RateTableProvider.RateTolerance)
        {
            issues.Add(CreateIssue(
                payslip,
                validationId,
                IssueSeverity.Critical,
                null, // No warning message for underpayment - evidence fields are used
                casualRate,
                actualRate,
                payslip.OrdinaryHours,
                $"Casual Rate {payslip.Classification}"
            ));

            // Don't check system rate if already underpaying
            return issues;
        }

        // Check 2: Is the system configured rate below minimum Casual rate?
        // (Actual pay is OK, but the configured rate is wrong - data risk)
        if (payslip.HourlyRate < casualRate - RateTableProvider.RateTolerance)
        {
            issues.Add(CreateIssue(
                payslip,
                validationId,
                IssueSeverity.Warning,
                $"System Casual rate ${payslip.HourlyRate:F2}/hr is below legal minimum ${casualRate:F2}/hr",
                casualRate,
                payslip.HourlyRate,
                payslip.OrdinaryHours,
                $"Casual Rate {payslip.Classification}"
            ));
        }

        return issues;
    }

    private PayrollIssue CreateIssue(
        Payslip payslip,
        Guid validationId,
        IssueSeverity severity,
        string? warningMessage,
        decimal expectedValue,
        decimal actualValue,
        decimal affectedUnits,
        string contextLabel)
    {
        // Warning has no financial impact (ImpactAmount = 0)
        var impactAmount = severity == IssueSeverity.Warning
            ? 0
            : (expectedValue - actualValue) * affectedUnits;

        return new PayrollIssue
        {
            OrganizationId = payslip.OrganizationId,
            PayrollValidationId = validationId,
            PayslipId = payslip.Id,
            EmployeeId = payslip.EmployeeId,
            CategoryType = IssueCategory.CasualLoading,
            Severity = severity,
            WarningMessage = warningMessage,
            ExpectedValue = expectedValue,
            ActualValue = actualValue,
            AffectedUnits = affectedUnits,
            UnitType = "Hour",
            ContextLabel = contextLabel,
            ImpactAmount = impactAmount
        };
    }
}
