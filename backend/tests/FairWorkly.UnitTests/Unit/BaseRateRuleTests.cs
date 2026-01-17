using FluentAssertions;
using FairWorkly.Application.Payroll.Services.ComplianceEngine;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Payroll.Entities;

namespace FairWorkly.UnitTests.Unit;

public class BaseRateRuleTests
{
    private readonly BaseRateRule _rule;
    private readonly Guid _validationId = Guid.NewGuid();

    public BaseRateRuleTests()
    {
        _rule = new BaseRateRule();
    }

    [Fact]
    public void RuleName_ShouldBeBaseRateCheck()
    {
        _rule.RuleName.Should().Be("Base Rate Check");
    }

    [Theory]
    [InlineData(1, 26.55, 38.00, 1008.90)]  // Level 1 at exact minimum
    [InlineData(2, 27.16, 20.00, 543.20)]   // Level 2 at exact minimum
    [InlineData(3, 28.00, 25.00, 700.00)]   // Level 3 above minimum (27.58)
    [InlineData(5, 30.00, 38.00, 1140.00)]  // Level 5 above minimum (29.27)
    public void Evaluate_WhenRateAtOrAboveMinimum_ShouldReturnNoIssues(
        int level, decimal hourlyRate, decimal hours, decimal pay)
    {
        // Arrange
        var payslip = CreatePayslip($"Level {level}", hourlyRate, hours, pay);

        // Act
        var issues = _rule.Evaluate(payslip, _validationId);

        // Assert
        issues.Should().BeEmpty();
    }

    [Theory]
    [InlineData(1, 25.00, 40.00, 1000.00, 26.55)]  // Level 1: $25/hr < $26.55
    [InlineData(2, 26.00, 30.00, 780.00, 27.16)]   // Level 2: $26/hr < $27.16
    [InlineData(5, 20.00, 38.00, 760.00, 29.27)]   // Level 5: $20/hr < $29.27
    public void Evaluate_WhenActualRateBelowMinimum_ShouldReturnCriticalIssue(
        int level, decimal hourlyRate, decimal hours, decimal pay, decimal minimumRate)
    {
        // Arrange
        var payslip = CreatePayslip($"Level {level}", hourlyRate, hours, pay);
        var actualRate = pay / hours;

        // Act
        var issues = _rule.Evaluate(payslip, _validationId);

        // Assert
        issues.Should().HaveCount(1);
        var issue = issues[0];
        issue.Severity.Should().Be(IssueSeverity.Critical);
        issue.CategoryType.Should().Be(IssueCategory.BaseRate);
        issue.ExpectedValue.Should().Be(minimumRate);
        issue.ActualValue.Should().BeApproximately(actualRate, 0.01m);
        issue.AffectedUnits.Should().Be(hours);
        issue.ImpactAmount.Should().BeApproximately((minimumRate - actualRate) * hours, 0.01m);
    }

    [Fact]
    public void Evaluate_WhenSystemRateBelowMinimumButActualPayCorrect_ShouldReturnWarning()
    {
        // Arrange: System rate $25/hr (wrong), but actual pay is correct ($26.55 × 38 = $1008.90)
        var payslip = CreatePayslip("Level 1", hourlyRate: 25.00m, hours: 38.00m, pay: 1008.90m);

        // Act
        var issues = _rule.Evaluate(payslip, _validationId);

        // Assert
        issues.Should().HaveCount(1);
        var issue = issues[0];
        issue.Severity.Should().Be(IssueSeverity.Warning);
        issue.WarningMessage.Should().Contain("System rate");
        issue.ExpectedValue.Should().Be(26.55m);
        issue.ActualValue.Should().Be(25.00m);
        issue.ImpactAmount.Should().Be(0); // Warning has no financial impact
    }

    [Fact]
    public void Evaluate_WhenZeroOrdinaryHours_ShouldReturnNoIssues()
    {
        // Arrange
        var payslip = CreatePayslip("Level 1", 26.55m, hours: 0, pay: 0);

        // Act
        var issues = _rule.Evaluate(payslip, _validationId);

        // Assert
        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenCasualEmployee_ShouldStillCheckAgainstPermanentRate()
    {
        // Arrange: Casual at $26.55 (Permanent rate, not Casual rate of $33.19)
        // This should pass BaseRateRule but fail CasualLoadingRule
        var payslip = CreatePayslip("Level 1", 26.55m, 20.00m, 531.00m, EmploymentType.Casual);

        // Act
        var issues = _rule.Evaluate(payslip, _validationId);

        // Assert: BaseRateRule only checks against Permanent Rate minimum
        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenRateWithinTolerance_ShouldReturnNoIssues()
    {
        // Arrange: Rate is $26.54, minimum is $26.55, within $0.01 tolerance
        var payslip = CreatePayslip("Level 1", 26.54m, 40.00m, 1061.60m);

        // Act
        var issues = _rule.Evaluate(payslip, _validationId);

        // Assert: $26.54 is within tolerance of $26.55
        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenLevel8Employee_ShouldUseCorrectMinimum()
    {
        // Arrange: Level 8 minimum is $32.45
        var payslip = CreatePayslip("Level 8", 32.45m, 38.00m, 1233.10m);

        // Act
        var issues = _rule.Evaluate(payslip, _validationId);

        // Assert
        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_Level4_WhenAtMinimum_ShouldReturnNoIssues()
    {
        // Arrange: Level 4 minimum is $28.12
        var payslip = CreatePayslip("Level 4", 28.12m, 38.00m, 1068.56m);

        // Act
        var issues = _rule.Evaluate(payslip, _validationId);

        // Assert
        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_Level6_WhenBelowMinimum_ShouldReturnCritical()
    {
        // Arrange: Level 6 minimum is $29.70, paying $28.00/hr
        var payslip = CreatePayslip("Level 6", 28.00m, 40.00m, 1120.00m);
        var actualRate = 1120.00m / 40.00m; // $28.00

        // Act
        var issues = _rule.Evaluate(payslip, _validationId);

        // Assert
        issues.Should().HaveCount(1);
        var issue = issues[0];
        issue.Severity.Should().Be(IssueSeverity.Critical);
        issue.CategoryType.Should().Be(IssueCategory.BaseRate);
        issue.ExpectedValue.Should().Be(29.70m);
        issue.ActualValue.Should().BeApproximately(actualRate, 0.01m);
        issue.ImpactAmount.Should().BeApproximately((29.70m - actualRate) * 40.00m, 0.05m);
    }

    [Fact]
    public void Evaluate_Level7_WhenAtMinimum_ShouldReturnNoIssues()
    {
        // Arrange: Level 7 minimum is $31.19
        var payslip = CreatePayslip("Level 7", 31.19m, 38.00m, 1185.22m);

        // Act
        var issues = _rule.Evaluate(payslip, _validationId);

        // Assert
        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_Level8_WhenBelowMinimum_ShouldReturnCritical()
    {
        // Arrange: Level 8 minimum is $32.45, paying $30.00/hr
        var payslip = CreatePayslip("Level 8", 30.00m, 40.00m, 1200.00m);
        var actualRate = 1200.00m / 40.00m; // $30.00

        // Act
        var issues = _rule.Evaluate(payslip, _validationId);

        // Assert
        issues.Should().HaveCount(1);
        var issue = issues[0];
        issue.Severity.Should().Be(IssueSeverity.Critical);
        issue.CategoryType.Should().Be(IssueCategory.BaseRate);
        issue.ExpectedValue.Should().Be(32.45m);
        issue.ActualValue.Should().BeApproximately(actualRate, 0.01m);
        issue.ImpactAmount.Should().BeApproximately((32.45m - actualRate) * 40.00m, 0.05m);
    }

    private Payslip CreatePayslip(
        string classification,
        decimal hourlyRate,
        decimal hours,
        decimal pay,
        EmploymentType employmentType = EmploymentType.FullTime)
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
            EmployeeName = "Test Employee",
            EmployeeNumber = "EMP001",
            AwardType = AwardType.GeneralRetailIndustryAward2020,
            PayPeriodStart = DateTimeOffset.Now.AddDays(-7),
            PayPeriodEnd = DateTimeOffset.Now,
            PayDate = DateTimeOffset.Now
        };
    }
}
