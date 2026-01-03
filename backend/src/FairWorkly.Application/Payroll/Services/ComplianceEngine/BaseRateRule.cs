using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Payroll.Entities;

namespace FairWorkly.Application.Payroll.Services.ComplianceEngine;

/// <summary>
/// Validates that employees are paid at least the minimum base rate
/// Applies to ALL employee types (Permanent and Casual)
/// Uses Permanent Rate as the minimum threshold for all
/// </summary>
public class BaseRateRule : IComplianceRule
{
    public string RuleName => "Base Rate Check";

    public List<PayrollIssue> Evaluate(Payslip payslip, Guid validationId)
    {
        var issues = new List<PayrollIssue>();

        // Skip if no ordinary hours worked
        if (payslip.OrdinaryHours <= 0)
        {
            return issues;
        }

        // Get the minimum legal rate for this level
        var level = RateTableProvider.ParseLevel(payslip.Classification);
        var minimumRate = RateTableProvider.GetPermanentRate(level);

        if (minimumRate <= 0)
        {
            // Invalid level - should have been caught by pre-validation
            return issues;
        }

        // Calculate actual hourly rate from pay
        var actualRate = payslip.OrdinaryPay / payslip.OrdinaryHours;

        // Check 1: Is the actual paid rate below minimum?
        if (actualRate < minimumRate - RateTableProvider.RateTolerance)
        {
            issues.Add(CreateIssue(
                payslip,
                validationId,
                IssueSeverity.Critical,
                null, // No warning message for underpayment - evidence fields are used
                minimumRate,
                actualRate,
                payslip.OrdinaryHours,
                $"Retail Award {payslip.Classification}"
            ));

            // Don't check system rate if already underpaying
            return issues;
        }

        // Check 2: Is the system configured rate below minimum?
        // (Actual pay is OK, but the configured rate is wrong - data risk)
        if (payslip.HourlyRate < minimumRate - RateTableProvider.RateTolerance)
        {
            issues.Add(CreateIssue(
                payslip,
                validationId,
                IssueSeverity.Warning,
                $"System rate ${payslip.HourlyRate:F2}/hr is below legal minimum ${minimumRate:F2}/hr",
                minimumRate,
                payslip.HourlyRate,
                payslip.OrdinaryHours,
                $"Retail Award {payslip.Classification}"
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
        var impactAmount = (expectedValue - actualValue) * affectedUnits;

        return new PayrollIssue
        {
            OrganizationId = payslip.OrganizationId,
            PayrollValidationId = validationId,
            PayslipId = payslip.Id,
            EmployeeId = payslip.EmployeeId,
            CategoryType = IssueCategory.BaseRate,
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
