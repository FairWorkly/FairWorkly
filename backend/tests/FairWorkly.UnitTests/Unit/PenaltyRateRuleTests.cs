using FairWorkly.Application.Payroll.Services.ComplianceEngine;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Payroll.Entities;
using FluentAssertions;

namespace FairWorkly.UnitTests.Unit;

public class PenaltyRateRuleTests
{
    private readonly PenaltyRateRule _rule;
    private readonly Guid _validationId = Guid.NewGuid();

    // Level 2 Permanent Rate = $27.16
    private const decimal Level2Rate = 27.16m;

    public PenaltyRateRuleTests()
    {
        _rule = new PenaltyRateRule();
    }

    [Fact]
    public void RuleName_ShouldBePenaltyRateCheck()
    {
        _rule.RuleName.Should().Be("Penalty Rate Check");
    }

    #region Saturday Tests

    [Fact]
    public void Evaluate_SaturdayFullTime_WhenPaidCorrectly_ShouldReturnNoIssues()
    {
        // Level 2 FullTime Saturday: $27.16 × 1.25 × 8 = $271.60
        var payslip = CreatePayslip(
            EmploymentType.FullTime,
            saturdayHours: 8m,
            saturdayPay: 271.60m
        );

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_SaturdayFullTime_WhenUnderpaid_ShouldReturnError()
    {
        // Expected: $27.16 × 1.25 × 8 = $271.60, Actual: $217.28 (20% short)
        var payslip = CreatePayslip(
            EmploymentType.FullTime,
            saturdayHours: 8m,
            saturdayPay: 217.28m
        );

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().HaveCount(1);
        var issue = issues[0];
        issue.Severity.Should().Be(IssueSeverity.Error);
        issue.CategoryType.Should().Be(IssueCategory.PenaltyRate);
        issue.ContextLabel.Should().Contain("Saturday");
        issue.ExpectedValue.Should().BeApproximately(33.95m, 0.01m);
        issue.ActualValue.Should().BeApproximately(27.16m, 0.01m);
    }

    [Fact]
    public void Evaluate_SaturdayCasual_WhenUnderpaid_ShouldUseHigherMultiplier()
    {
        // Casual Saturday: $27.16 × 1.50 × 5 = $203.70 (NOT $27.16 × 1.25)
        // Actual: $132.75 (uses wrong multiplier)
        var payslip = CreatePayslip(EmploymentType.Casual, saturdayHours: 5m, saturdayPay: 132.75m);

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().HaveCount(1);
        var issue = issues[0];
        issue.ExpectedValue.Should().BeApproximately(40.74m, 0.01m);
    }

    #endregion

    #region Sunday Tests

    [Fact]
    public void Evaluate_SundayPartTime_WhenPaidCorrectly_ShouldReturnNoIssues()
    {
        // Level 2 PartTime Sunday: $27.16 × 1.50 × 6 = $244.44
        var payslip = CreatePayslip(EmploymentType.PartTime, sundayHours: 6m, sundayPay: 244.44m);

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_SundayCasual_WhenUnderpaid_ShouldReturnError()
    {
        // Casual Sunday: $27.16 × 1.75 × 4 = $190.12
        var payslip = CreatePayslip(EmploymentType.Casual, sundayHours: 4m, sundayPay: 150.00m);

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().HaveCount(1);
        issues[0].ContextLabel.Should().Contain("Sunday");
    }

    #endregion

    #region Public Holiday Tests

    [Fact]
    public void Evaluate_PublicHolidayFullTime_WhenPaidCorrectly_ShouldReturnNoIssues()
    {
        // Level 2 FullTime PH: $27.16 × 2.25 × 8 = $488.88
        var payslip = CreatePayslip(EmploymentType.FullTime, phHours: 8m, phPay: 488.88m);

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_PublicHolidayCasual_WhenUnderpaid_ShouldReturnError()
    {
        // Casual PH: $27.16 × 2.50 × 4 = $271.60
        var payslip = CreatePayslip(EmploymentType.Casual, phHours: 4m, phPay: 200.00m);

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().HaveCount(1);
        var issue = issues[0];
        issue.ContextLabel.Should().Contain("Public Holiday");
        issue.ExpectedValue.Should().BeApproximately(67.90m, 0.01m);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Evaluate_WhenNoWeekendHours_ShouldReturnNoIssues()
    {
        var payslip = CreatePayslip(EmploymentType.FullTime);
        // No weekend/PH hours set

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenWithinTolerance_ShouldReturnNoIssues()
    {
        // Expected: $271.60, Actual: $271.56 (only $0.04 under, within $0.05 tolerance)
        var payslip = CreatePayslip(
            EmploymentType.FullTime,
            saturdayHours: 8m,
            saturdayPay: 271.56m
        );

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenJustOverTolerance_ShouldReturnError()
    {
        // Expected: $271.60, Actual: $271.54 ($0.06 under, over $0.05 tolerance)
        var payslip = CreatePayslip(
            EmploymentType.FullTime,
            saturdayHours: 8m,
            saturdayPay: 271.54m
        );

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().HaveCount(1);
    }

    [Fact]
    public void Evaluate_MultipleViolations_ShouldReturnMultipleIssues()
    {
        // All three penalty types underpaid
        var payslip = CreatePayslip(
            EmploymentType.FullTime,
            saturdayHours: 8m,
            saturdayPay: 200m, // Under
            sundayHours: 6m,
            sundayPay: 200m, // Under
            phHours: 4m,
            phPay: 200m // Under
        );

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().HaveCount(3);
        issues.Should().Contain(i => i.ContextLabel!.Contains("Saturday"));
        issues.Should().Contain(i => i.ContextLabel!.Contains("Sunday"));
        issues.Should().Contain(i => i.ContextLabel!.Contains("Public Holiday"));
    }

    [Fact]
    public void Evaluate_FixedTermEmployee_ShouldUsePermanentMultipliers()
    {
        // FixedTerm uses Permanent multipliers (1.25x Saturday)
        // Expected: $27.16 × 1.25 × 8 = $271.60
        var payslip = CreatePayslip(EmploymentType.FixedTerm, saturdayHours: 8m, saturdayPay: 200m);

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().HaveCount(1);
        issues[0].ExpectedValue.Should().BeApproximately(33.95m, 0.01m);
    }

    #endregion

    #region Negative Pay Tests

    [Fact]
    public void Evaluate_WhenSaturdayPayNegative_ShouldReturnWarning()
    {
        // Arrange: Negative Saturday pay indicates correction/reversal entry
        var payslip = CreatePayslip(
            EmploymentType.FullTime,
            saturdayHours: 8m,
            saturdayPay: -271.60m
        );

        // Act
        var issues = _rule.Evaluate(payslip, _validationId);

        // Assert
        issues.Should().HaveCount(1);
        var issue = issues[0];
        issue.Severity.Should().Be(IssueSeverity.Warning);
        issue.CategoryType.Should().Be(IssueCategory.PenaltyRate);
        issue.WarningMessage.Should().Contain("Negative");
        issue.WarningMessage.Should().Contain("Saturday Pay");
    }

    [Fact]
    public void Evaluate_WhenSundayPayNegative_ShouldReturnWarning()
    {
        // Arrange: Negative Sunday pay indicates correction/reversal entry
        var payslip = CreatePayslip(EmploymentType.FullTime, sundayHours: 6m, sundayPay: -244.44m);

        // Act
        var issues = _rule.Evaluate(payslip, _validationId);

        // Assert
        issues.Should().HaveCount(1);
        var issue = issues[0];
        issue.Severity.Should().Be(IssueSeverity.Warning);
        issue.CategoryType.Should().Be(IssueCategory.PenaltyRate);
        issue.WarningMessage.Should().Contain("Negative");
        issue.WarningMessage.Should().Contain("Sunday Pay");
    }

    [Fact]
    public void Evaluate_WhenPublicHolidayPayNegative_ShouldReturnWarning()
    {
        // Arrange: Negative public holiday pay indicates correction/reversal entry
        var payslip = CreatePayslip(EmploymentType.FullTime, phHours: 8m, phPay: -488.88m);

        // Act
        var issues = _rule.Evaluate(payslip, _validationId);

        // Assert
        issues.Should().HaveCount(1);
        var issue = issues[0];
        issue.Severity.Should().Be(IssueSeverity.Warning);
        issue.CategoryType.Should().Be(IssueCategory.PenaltyRate);
        issue.WarningMessage.Should().Contain("Negative");
        issue.WarningMessage.Should().Contain("Public Holiday Pay");
    }

    [Fact]
    public void Evaluate_WhenMixedPositiveAndNegativePay_ShouldHandleSeparately()
    {
        // Arrange: Saturday positive (underpaid), Sunday negative (reversal)
        // Saturday: Expected $271.60, Actual $200 (underpaid - should return Error)
        // Sunday: Negative pay (reversal - should return Warning)
        var payslip = CreatePayslip(
            EmploymentType.FullTime,
            saturdayHours: 8m,
            saturdayPay: 200m,
            sundayHours: 6m,
            sundayPay: -244.44m
        );

        // Act
        var issues = _rule.Evaluate(payslip, _validationId);

        // Assert: Should have 2 issues - one Error for Saturday, one Warning for Sunday
        issues.Should().HaveCount(2);
        issues
            .Should()
            .Contain(i =>
                i.Severity == IssueSeverity.Error && i.ContextLabel!.Contains("Saturday")
            );
        issues
            .Should()
            .Contain(i =>
                i.Severity == IssueSeverity.Warning
                && i.WarningMessage!.Contains("Negative")
                && i.WarningMessage.Contains("Sunday Pay")
            );
    }

    #endregion

    private Payslip CreatePayslip(
        EmploymentType employmentType,
        decimal saturdayHours = 0,
        decimal saturdayPay = 0,
        decimal sundayHours = 0,
        decimal sundayPay = 0,
        decimal phHours = 0,
        decimal phPay = 0
    )
    {
        return new Payslip
        {
            Id = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            Classification = "Level 2",
            HourlyRate = Level2Rate,
            EmploymentType = employmentType,
            OrdinaryHours = 38m,
            OrdinaryPay = 1032.08m,
            SaturdayHours = saturdayHours,
            SaturdayPay = saturdayPay,
            SundayHours = sundayHours,
            SundayPay = sundayPay,
            PublicHolidayHours = phHours,
            PublicHolidayPay = phPay,
            GrossPay = 1032.08m + saturdayPay + sundayPay + phPay,
            Superannuation = 123.85m,
            EmployeeName = "Test Employee",
            EmployeeNumber = "EMP001",
            AwardType = AwardType.GeneralRetailIndustryAward2020,
            PayPeriodStart = DateTimeOffset.Now.AddDays(-7),
            PayPeriodEnd = DateTimeOffset.Now,
            PayDate = DateTimeOffset.Now,
        };
    }
}
