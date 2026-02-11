using FairWorkly.Application.Payroll.Services.ComplianceEngine;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Payroll.Entities;
using FluentAssertions;

namespace FairWorkly.UnitTests.Unit;

public class SuperannuationRuleTests
{
    private readonly SuperannuationRule _rule;
    private readonly Guid _validationId = Guid.NewGuid();

    public SuperannuationRuleTests()
    {
        _rule = new SuperannuationRule();
    }

    [Fact]
    public void RuleName_ShouldBeSuperannuationCheck()
    {
        _rule.RuleName.Should().Be("Superannuation Check");
    }

    #region Correct Superannuation Tests

    [Fact]
    public void Evaluate_WhenSuperannuationIsExact12Percent_ShouldReturnNoIssues()
    {
        // Gross: $1000, Expected Super: $120
        var payslip = CreatePayslip(grossPay: 1000m, superannuation: 120m);

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenSuperannuationAbove12Percent_ShouldReturnNoIssues()
    {
        // Gross: $1000, Expected: $120, Actual: $150 (overpaid is fine)
        var payslip = CreatePayslip(grossPay: 1000m, superannuation: 150m);

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().BeEmpty();
    }

    [Theory]
    [InlineData(1000, 120)] // Exact 12%
    [InlineData(2500, 300)] // Exact 12%
    [InlineData(500, 60)] // Exact 12%
    [InlineData(1500, 200)] // Above 12% (13.33%)
    public void Evaluate_WhenSuperannuationAtOrAbove12Percent_ShouldReturnNoIssues(
        decimal grossPay,
        decimal superannuation
    )
    {
        var payslip = CreatePayslip(grossPay: grossPay, superannuation: superannuation);

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().BeEmpty();
    }

    #endregion

    #region Underpayment Tests

    [Fact]
    public void Evaluate_WhenSuperannuationIsZero_ShouldReturnError()
    {
        // Gross: $1000, Expected: $120, Actual: $0
        var payslip = CreatePayslip(grossPay: 1000m, superannuation: 0m);

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().HaveCount(1);
        var issue = issues[0];
        issue.Severity.Should().Be(IssueSeverity.Error);
        issue.CategoryType.Should().Be(IssueCategory.Superannuation);
        issue.ExpectedValue.Should().Be(120m);
        issue.ActualValue.Should().Be(0m);
    }

    [Fact]
    public void Evaluate_WhenSuperannuationUnderpaid_ShouldReturnError()
    {
        // Gross: $2000, Expected: $240, Actual: $200
        var payslip = CreatePayslip(grossPay: 2000m, superannuation: 200m);

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().HaveCount(1);
        var issue = issues[0];
        issue.Severity.Should().Be(IssueSeverity.Error);
        issue.CategoryType.Should().Be(IssueCategory.Superannuation);
        issue.ExpectedValue.Should().Be(240m);
        issue.ActualValue.Should().Be(200m);
        issue.AffectedUnits.Should().Be(2000m); // Gross pay as affected units
    }

    [Theory]
    [InlineData(1000, 100)] // Underpaid by $20
    [InlineData(2000, 180)] // Underpaid by $60
    [InlineData(5000, 500)] // Underpaid by $100
    public void Evaluate_WhenSignificantlyUnderpaid_ShouldReturnError(
        decimal grossPay,
        decimal superannuation
    )
    {
        var payslip = CreatePayslip(grossPay: grossPay, superannuation: superannuation);
        var expectedSuper = grossPay * 0.12m;

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().HaveCount(1);
        issues[0].ExpectedValue.Should().Be(expectedSuper);
    }

    #endregion

    #region GrossPay Data Validity Tests

    [Fact]
    public void Evaluate_WhenGrossPayZeroWithWorkHours_ShouldReturnWarning()
    {
        // Has work hours but zero gross pay - data anomaly
        var payslip = CreatePayslip(grossPay: 0m, superannuation: 0m, ordinaryHours: 38m);

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().HaveCount(1);
        var issue = issues[0];
        issue.Severity.Should().Be(IssueSeverity.Warning);
        issue.CategoryType.Should().Be(IssueCategory.Superannuation);
        issue.WarningMessage.Should().Contain("Zero Gross Pay");
        issue.WarningMessage.Should().Contain("38");
    }

    [Theory]
    [InlineData(0, 8, 0, 0)] // Only Saturday hours
    [InlineData(0, 0, 5, 0)] // Only Sunday hours
    [InlineData(0, 0, 0, 8)] // Only Public Holiday hours
    public void Evaluate_WhenGrossPayZeroWithAnyWorkHours_ShouldReturnWarning(
        decimal ordinaryHours,
        decimal saturdayHours,
        decimal sundayHours,
        decimal phHours
    )
    {
        var payslip = CreatePayslip(
            grossPay: 0m,
            superannuation: 0m,
            ordinaryHours: ordinaryHours,
            saturdayHours: saturdayHours,
            sundayHours: sundayHours,
            phHours: phHours
        );

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().HaveCount(1);
        var issue = issues[0];
        issue.Severity.Should().Be(IssueSeverity.Warning);
        issue.CategoryType.Should().Be(IssueCategory.Superannuation);
        issue.WarningMessage.Should().Contain("Zero Gross Pay");
    }

    [Fact]
    public void Evaluate_WhenGrossPayZeroWithNoWorkHours_ShouldReturnNoIssues()
    {
        // No hours and no pay - unpaid period (e.g. unpaid leave), skip
        var payslip = CreatePayslip(grossPay: 0m, superannuation: 0m, ordinaryHours: 0m);

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenGrossPayNegative_ShouldReturnWarning()
    {
        // Negative gross pay indicates correction/reversal entry
        var payslip = CreatePayslip(grossPay: -500m, superannuation: -60m, ordinaryHours: 0m);

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().HaveCount(1);
        var issue = issues[0];
        issue.Severity.Should().Be(IssueSeverity.Warning);
        issue.CategoryType.Should().Be(IssueCategory.Superannuation);
        issue.WarningMessage.Should().Contain("Negative Gross Pay");
    }

    [Fact]
    public void Evaluate_WhenGrossPayPositive_ShouldContinueNormalValidation()
    {
        // Scenario A: Underpaid super (should trigger ERROR)
        var payslipUnderpaid = CreatePayslip(grossPay: 1000m, superannuation: 100m);
        var issuesA = _rule.Evaluate(payslipUnderpaid, _validationId);

        issuesA.Should().HaveCount(1);
        issuesA[0].Severity.Should().Be(IssueSeverity.Error);

        // Scenario B: Compliant super (should PASS)
        var payslipCompliant = CreatePayslip(grossPay: 1000m, superannuation: 120m);
        var issuesB = _rule.Evaluate(payslipCompliant, _validationId);

        issuesB.Should().BeEmpty();
    }

    #endregion

    #region Tolerance Tests

    [Fact]
    public void Evaluate_WhenWithinTolerance_ShouldReturnNoIssues()
    {
        // Gross: $1000, Expected: $120, Actual: $119.96 (only $0.04 under, within $0.05 tolerance)
        var payslip = CreatePayslip(grossPay: 1000m, superannuation: 119.96m);

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenJustOverTolerance_ShouldReturnError()
    {
        // Gross: $1000, Expected: $120, Actual: $119.94 ($0.06 under, over $0.05 tolerance)
        var payslip = CreatePayslip(grossPay: 1000m, superannuation: 119.94m);

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().HaveCount(1);
        issues[0].Severity.Should().Be(IssueSeverity.Error);
    }

    [Fact]
    public void Evaluate_WhenExactlyAtTolerance_ShouldReturnNoIssues()
    {
        // Gross: $1000, Expected: $120, Actual: $119.95 (exactly $0.05 under)
        var payslip = CreatePayslip(grossPay: 1000m, superannuation: 119.95m);

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().BeEmpty();
    }

    #endregion

    #region All Employee Types Tests

    [Theory]
    [InlineData(EmploymentType.FullTime)]
    [InlineData(EmploymentType.PartTime)]
    [InlineData(EmploymentType.Casual)]
    [InlineData(EmploymentType.FixedTerm)]
    public void Evaluate_AllEmployeeTypes_ShouldCheck12PercentRule(EmploymentType employmentType)
    {
        // All employee types should have 12% super
        var payslip = CreatePayslip(
            grossPay: 1000m,
            superannuation: 50m, // Underpaid
            employmentType: employmentType
        );

        var issues = _rule.Evaluate(payslip, _validationId);

        issues.Should().HaveCount(1);
        issues[0].ExpectedValue.Should().Be(120m);
    }

    #endregion

    private Payslip CreatePayslip(
        decimal grossPay,
        decimal superannuation,
        decimal ordinaryHours = 38m,
        decimal saturdayHours = 0m,
        decimal sundayHours = 0m,
        decimal phHours = 0m,
        EmploymentType employmentType = EmploymentType.FullTime
    )
    {
        return new Payslip
        {
            Id = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            Classification = "Level 2",
            HourlyRate = 27.16m,
            OrdinaryHours = ordinaryHours,
            OrdinaryPay = ordinaryHours * 27.16m,
            SaturdayHours = saturdayHours,
            SaturdayPay = 0m,
            SundayHours = sundayHours,
            SundayPay = 0m,
            PublicHolidayHours = phHours,
            PublicHolidayPay = 0m,
            GrossPay = grossPay,
            Superannuation = superannuation,
            EmploymentType = employmentType,
            EmployeeName = "Test Employee",
            EmployeeNumber = "EMP001",
            AwardType = AwardType.GeneralRetailIndustryAward2020,
            PayPeriodStart = DateTimeOffset.Now.AddDays(-7),
            PayPeriodEnd = DateTimeOffset.Now,
            PayDate = DateTimeOffset.Now,
        };
    }
}
