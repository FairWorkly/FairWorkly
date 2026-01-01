using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Payroll.Entities;

namespace FairWorkly.Application.Payroll.Services.ComplianceEngine;

/// <summary>
/// Validates that penalty rates (Saturday, Sunday, Public Holiday) are paid correctly
/// IMPORTANT: ALL employee types use Permanent Rate as the base for penalty calculations
/// Only the multiplier differs between Permanent and Casual employees
/// </summary>
public class PenaltyRateRule : IComplianceRule
{
    public string RuleName => "Penalty Rate Check";

    public List<PayrollIssue> Evaluate(Payslip payslip, Guid validationId)
    {
        var issues = new List<PayrollIssue>();

        var level = RateTableProvider.ParseLevel(payslip.Classification);
        var baseRate = RateTableProvider.GetPermanentRate(level);

        if (baseRate <= 0)
        {
            // Invalid level - should have been caught by pre-validation
            return issues;
        }

        var isCasual = payslip.EmploymentType == EmploymentType.Casual;

        // Check Saturday
        if (payslip.SaturdayHours > 0)
        {
            var multiplier = isCasual
                ? RateTableProvider.CasualMultipliers.Saturday
                : RateTableProvider.PermanentMultipliers.Saturday;

            var expectedPay = baseRate * multiplier * payslip.SaturdayHours;

            if (payslip.SaturdayPay < expectedPay - RateTableProvider.PayTolerance)
            {
                issues.Add(CreateIssue(
                    payslip,
                    validationId,
                    "Saturday",
                    expectedPay,
                    payslip.SaturdayPay,
                    payslip.SaturdayHours,
                    multiplier
                ));
            }
        }

        // Check Sunday
        if (payslip.SundayHours > 0)
        {
            var multiplier = isCasual
                ? RateTableProvider.CasualMultipliers.Sunday
                : RateTableProvider.PermanentMultipliers.Sunday;

            var expectedPay = baseRate * multiplier * payslip.SundayHours;

            if (payslip.SundayPay < expectedPay - RateTableProvider.PayTolerance)
            {
                issues.Add(CreateIssue(
                    payslip,
                    validationId,
                    "Sunday",
                    expectedPay,
                    payslip.SundayPay,
                    payslip.SundayHours,
                    multiplier
                ));
            }
        }

        // Check Public Holiday
        if (payslip.PublicHolidayHours > 0)
        {
            var multiplier = isCasual
                ? RateTableProvider.CasualMultipliers.PublicHoliday
                : RateTableProvider.PermanentMultipliers.PublicHoliday;

            var expectedPay = baseRate * multiplier * payslip.PublicHolidayHours;

            if (payslip.PublicHolidayPay < expectedPay - RateTableProvider.PayTolerance)
            {
                issues.Add(CreateIssue(
                    payslip,
                    validationId,
                    "Public Holiday",
                    expectedPay,
                    payslip.PublicHolidayPay,
                    payslip.PublicHolidayHours,
                    multiplier
                ));
            }
        }

        return issues;
    }

    private PayrollIssue CreateIssue(
        Payslip payslip,
        Guid validationId,
        string dayType,
        decimal expectedPay,
        decimal actualPay,
        decimal hours,
        decimal multiplier)
    {
        var shortfall = expectedPay - actualPay;

        return new PayrollIssue
        {
            OrganizationId = payslip.OrganizationId,
            PayrollValidationId = validationId,
            PayslipId = payslip.Id,
            EmployeeId = payslip.EmployeeId,
            CheckType = RuleName,
            Severity = IssueSeverity.Error,
            Description = $"{dayType} penalty underpayment: Paid ${actualPay:F2}, Expected ${expectedPay:F2}",
            ExpectedValue = expectedPay,
            ActualValue = actualPay,
            AffectedUnits = hours,
            UnitType = "$",
            ContextLabel = $"{dayType} ({multiplier:P0} rate)"
        };
    }
}
