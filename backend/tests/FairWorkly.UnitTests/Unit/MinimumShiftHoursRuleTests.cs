using FairWorkly.Domain.Roster.Rules;
using FairWorkly.Domain.Roster.Parameters;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Employees.Entities;
using FairWorkly.Domain.Roster.Entities;
using FairWorkly.Domain.Roster.Enums;
using FluentAssertions;

namespace FairWorkly.UnitTests.Unit;

public class MinimumShiftHoursRuleTests
{
    private static readonly IRosterRuleParametersProvider ParametersProvider =
        new AwardRosterRuleParametersProvider();

    private readonly MinimumShiftHoursRule _rule = new(ParametersProvider);
    private readonly Guid _validationId = Guid.NewGuid();

    [Fact]
    public void CheckType_ShouldBeMinimumShiftHours()
    {
        _rule.CheckType.Should().Be(RosterCheckType.MinimumShiftHours);
    }

    [Fact]
    public void Evaluate_WhenPartTimeShiftBelowMinimum_ShouldReturnIssue()
    {
        var shift = CreateShift(EmploymentType.PartTime, startHour: 9, endHour: 11, endMinute: 30);

        var issues = _rule.Evaluate(new[] { shift }, _validationId);

        issues.Should().HaveCount(1);
        var issue = issues[0];
        issue.Severity.Should().Be(IssueSeverity.Error);
        issue.CheckType.Should().Be(RosterCheckType.MinimumShiftHours);
        issue.ExpectedValue.Should().Be(3m);
        issue.ActualValue.Should().BeApproximately(2.5m, 0.01m);
    }

    [Fact]
    public void Evaluate_WhenPartTimeShiftAtMinimum_ShouldReturnNoIssues()
    {
        var shift = CreateShift(EmploymentType.PartTime, startHour: 9, endHour: 12);

        var issues = _rule.Evaluate(new[] { shift }, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenFullTimeShortShift_ShouldReturnNoIssues()
    {
        var shift = CreateShift(EmploymentType.FullTime, startHour: 9, endHour: 10);

        var issues = _rule.Evaluate(new[] { shift }, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenCasualShiftBelowMinimum_ShouldReturnIssue()
    {
        var shift = CreateShift(EmploymentType.Casual, startHour: 9, endHour: 11);

        var issues = _rule.Evaluate(new[] { shift }, _validationId);

        issues.Should().HaveCount(1);
        issues[0].Severity.Should().Be(IssueSeverity.Error);
        issues[0].ExpectedValue.Should().Be(3m);
        issues[0].ActualValue.Should().Be(2m);
    }

    [Fact]
    public void Evaluate_WhenCasualShiftAtMinimum_ShouldReturnNoIssues()
    {
        var shift = CreateShift(EmploymentType.Casual, startHour: 9, endHour: 12);

        var issues = _rule.Evaluate(new[] { shift }, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenPartTimeShiftJustBelowMinimum_ShouldReturnIssue()
    {
        // 2.99 hours - just under 3 hour minimum
        var shift = CreateShift(EmploymentType.PartTime, startHour: 9, endHour: 11, endMinute: 59);

        var issues = _rule.Evaluate(new[] { shift }, _validationId);

        issues.Should().HaveCount(1);
        issues[0].ActualValue.Should().BeApproximately(2.98m, 0.02m);
    }

    [Fact]
    public void Evaluate_WhenMultipleShifts_ShouldEvaluateEach()
    {
        var validShift = CreateShift(EmploymentType.PartTime, startHour: 9, endHour: 13);
        var invalidShift = CreateShift(EmploymentType.Casual, startHour: 14, endHour: 16);

        var issues = _rule.Evaluate(new[] { validShift, invalidShift }, _validationId);

        issues.Should().HaveCount(1);
        issues[0].ActualValue.Should().Be(2m);
    }

    [Fact]
    public void Evaluate_WhenShiftHasNoEmployee_ShouldSkip()
    {
        var shift = new Shift
        {
            Id = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            RosterId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            Employee = null!,
            Date = new DateTime(2026, 1, 6),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0),
            HasMealBreak = false,
        };

        var issues = _rule.Evaluate(new[] { shift }, _validationId);

        issues.Should().BeEmpty();
    }

    private static Shift CreateShift(
        EmploymentType employmentType,
        int startHour,
        int endHour,
        int endMinute = 0
    )
    {
        var organizationId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var employee = new Employee
        {
            Id = employeeId,
            OrganizationId = organizationId,
            FirstName = "Test",
            LastName = "Employee",
            Email = "test@example.com",
            JobTitle = "Staff",
            EmploymentType = employmentType,
            StartDate = new DateTime(2025, 1, 1),
            AwardType = AwardType.GeneralRetailIndustryAward2020,
            AwardLevelNumber = 1,
        };

        return new Shift
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            RosterId = Guid.NewGuid(),
            EmployeeId = employeeId,
            Employee = employee,
            Date = new DateTime(2026, 1, 6),
            StartTime = new TimeSpan(startHour, 0, 0),
            EndTime = new TimeSpan(endHour, endMinute, 0),
            HasMealBreak = false,
        };
    }
}
