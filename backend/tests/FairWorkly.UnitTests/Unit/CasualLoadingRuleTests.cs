using FairWorkly.Application.Payroll.Services.ComplianceEngine;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Payroll.Entities;
using FluentAssertions;

namespace FairWorkly.UnitTests.Unit;

public class CasualLoadingRuleTests
{
    private readonly CasualLoadingRule _rule;
    private readonly Guid _validationId = Guid.NewGuid();

    // Level 2 Rates
    private const decimal Level2PermanentRate = 27.16m;
    private const decimal Level2CasualRate = 33.95m; // 27.16 × 1.25

    public CasualLoadingRuleTests()
    {
        _rule = new CasualLoadingRule();
    }

    [Fact]
    public void RuleName_ShouldBeCasualLoadingCheck()
    {
        _rule.RuleName.Should().Be("Casual Loading Check");
    }

    #region Non-Casual Employee Tests

    [Theory]
    [InlineData(EmploymentType.FullTime)]
    [InlineData(EmploymentType.PartTime)]
    [InlineData(EmploymentType.FixedTerm)]
    public void Evaluate_NonCasualEmployee_ShouldReturnNoIssues(EmploymentType employmentType)
    {
        // Non-casual employees should be skipped entirely
        var payslip = CreatePayslip(employmentType, hourlyRate: 27.16m, hours: 38m, pay: 1032.08m);

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().BeEmpty();
    }

    #endregion

    #region Casual Employee - Correct Pay Tests

    [Fact]
    public void Evaluate_CasualEmployee_WhenPaidAtCasualRate_ShouldReturnNoIssues()
    {
        // Level 2 Casual rate: $33.95/hr × 20hrs = $679.00
        var payslip = CreatePayslip(
            EmploymentType.Casual,
            hourlyRate: 33.95m,
            hours: 20m,
            pay: 679.00m
        );

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_CasualEmployee_WhenPaidAboveCasualRate_ShouldReturnNoIssues()
    {
        // Paid $35/hr (above $33.95 minimum)
        var payslip = CreatePayslip(
            EmploymentType.Casual,
            hourlyRate: 35.00m,
            hours: 20m,
            pay: 700.00m
        );

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().BeEmpty();
    }

    #endregion

    #region Casual Employee - Underpayment Tests

    [Fact]
    public void Evaluate_CasualEmployee_WhenPaidAtPermanentRate_ShouldReturnCriticalIssue()
    {
        // Paid at Permanent rate ($27.16) instead of Casual rate ($33.95)
        var payslip = CreatePayslip(
            EmploymentType.Casual,
            hourlyRate: 27.16m,
            hours: 20m,
            pay: 543.20m
        );
        var actualRate = 543.20m / 20m; // $27.16

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().HaveCount(1);
        var issue = issues[0];
        issue.Severity.Should().Be(IssueSeverity.Critical);
        issue.CategoryType.Should().Be(IssueCategory.CasualLoading);
        issue.ExpectedValue.Should().Be(Level2CasualRate);
        issue.ActualValue.Should().BeApproximately(actualRate, 0.01m);
    }

    [Fact]
    public void Evaluate_CasualEmployee_WhenSignificantlyUnderpaid_ShouldReturnCriticalIssue()
    {
        // Paid only $25/hr (well below $33.95)
        var payslip = CreatePayslip(
            EmploymentType.Casual,
            hourlyRate: 25.00m,
            hours: 15m,
            pay: 375.00m
        );

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().HaveCount(1);
        issues[0].Severity.Should().Be(IssueSeverity.Critical);
        issues[0].AffectedUnits.Should().Be(15m);
    }

    #endregion

    #region System Rate Warning Tests

    [Fact]
    public void Evaluate_CasualEmployee_WhenSystemRateBelowMinimumButActualPayCorrect_ShouldReturnWarning()
    {
        // System rate is $30/hr (wrong), but actual pay is correct ($33.95 × 20 = $679)
        var payslip = CreatePayslip(
            EmploymentType.Casual,
            hourlyRate: 30.00m,
            hours: 20m,
            pay: 679.00m
        );

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().HaveCount(1);
        var issue = issues[0];
        issue.Severity.Should().Be(IssueSeverity.Warning);
        issue.WarningMessage.Should().Contain("System Casual rate");
        issue.ExpectedValue.Should().Be(Level2CasualRate);
        issue.ActualValue.Should().Be(30.00m);
    }

    [Fact]
    public void Evaluate_CasualEmployee_WhenBothSystemAndActualRatesCorrect_ShouldReturnNoIssues()
    {
        // Both system rate and actual pay are correct
        var payslip = CreatePayslip(
            EmploymentType.Casual,
            hourlyRate: 33.95m,
            hours: 20m,
            pay: 679.00m
        );

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().BeEmpty();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Evaluate_CasualEmployee_WhenZeroOrdinaryHours_ShouldReturnNoIssues()
    {
        var payslip = CreatePayslip(EmploymentType.Casual, hourlyRate: 27.16m, hours: 0m, pay: 0m);

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_CasualEmployee_WhenWithinTolerance_ShouldReturnNoIssues()
    {
        // Actual rate: $33.94 (only $0.01 under $33.95, within tolerance)
        var payslip = CreatePayslip(
            EmploymentType.Casual,
            hourlyRate: 33.94m,
            hours: 20m,
            pay: 678.80m
        );

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_CasualEmployee_WhenJustOverTolerance_ShouldReturnCriticalIssue()
    {
        // Actual rate: $33.93 ($0.02 under $33.95, over $0.01 tolerance)
        var payslip = CreatePayslip(
            EmploymentType.Casual,
            hourlyRate: 33.93m,
            hours: 20m,
            pay: 678.60m
        );

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().HaveCount(1);
        issues[0].Severity.Should().Be(IssueSeverity.Critical);
    }

    [Theory]
    [InlineData(1, 33.19)] // Level 1: 26.55 × 1.25
    [InlineData(3, 34.48)] // Level 3: 27.58 × 1.25
    [InlineData(5, 36.59)] // Level 5: 29.27 × 1.25
    [InlineData(8, 40.56)] // Level 8: 32.45 × 1.25
    public void Evaluate_CasualEmployee_DifferentLevels_ShouldUseCasualRate(
        int level,
        decimal expectedCasualRate
    )
    {
        // Pay at Permanent rate (underpaying)
        var permanentRate = RateTableProvider.GetPermanentRate(level);
        var payslip = CreatePayslip(
            EmploymentType.Casual,
            classification: $"Level {level}",
            hourlyRate: permanentRate,
            hours: 10m,
            pay: permanentRate * 10m
        );

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().HaveCount(1);
        issues[0].ExpectedValue.Should().BeApproximately(expectedCasualRate, 0.01m);
    }

    #endregion

    #region Negative Pay Tests

    [Fact]
    public void Evaluate_CasualEmployee_WhenOrdinaryPayNegative_ShouldReturnWarning()
    {
        // Arrange: Negative pay indicates correction/reversal entry for casual employee
        var payslip = CreatePayslip(
            EmploymentType.Casual,
            hourlyRate: 33.95m,
            hours: 20m,
            pay: -679.00m
        );

        // Act
        var issues = _rule.Evaluate(payslip, _validationId);

        // Assert
        issues.Should().HaveCount(1);
        var issue = issues[0];
        issue.Severity.Should().Be(IssueSeverity.Warning);
        issue.CategoryType.Should().Be(IssueCategory.CasualLoading);
        issue.WarningMessage.Should().Contain("Negative");
        issue.WarningMessage.Should().Contain("Ordinary Pay");
    }

    #endregion

    private Payslip CreatePayslip(
        EmploymentType employmentType,
        decimal hourlyRate,
        decimal hours,
        decimal pay,
        string classification = "Level 2"
    )
    {
        return new Payslip
        {
            Id = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            Classification = classification,
            HourlyRate = hourlyRate,
            OrdinaryHours = hours,
            OrdinaryPay = pay,
            EmploymentType = employmentType,
            GrossPay = pay,
            Superannuation = pay * 0.12m,
            EmployeeName = "Test Casual Employee",
            EmployeeNumber = "CAS001",
            AwardType = AwardType.GeneralRetailIndustryAward2020,
            PayPeriodStart = DateTimeOffset.Now.AddDays(-7),
            PayPeriodEnd = DateTimeOffset.Now,
            PayDate = DateTimeOffset.Now,
        };
    }
}
