using FairWorkly.Domain.Roster.Rules;
using FairWorkly.Domain.Roster.Parameters;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Employees.Entities;
using FairWorkly.Domain.Roster.Entities;
using FairWorkly.Domain.Roster.Enums;
using FluentAssertions;

namespace FairWorkly.UnitTests.Unit;

public class MealBreakRuleTests
{
    private static readonly IRosterRuleParametersProvider ParametersProvider =
        new AwardRosterRuleParametersProvider();

    private readonly MealBreakRule _rule = new(ParametersProvider);
    private readonly Guid _validationId = Guid.NewGuid();

    [Fact]
    public void CheckType_ShouldBeMealBreak()
    {
        _rule.CheckType.Should().Be(RosterCheckType.MealBreak);
    }

    [Fact]
    public void Evaluate_WhenShiftUnderThreshold_ShouldReturnNoIssues()
    {
        var shift = CreateShift(durationHours: 5m, hasMealBreak: false);

        var issues = _rule.Evaluate(new[] { shift }, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenNoMealBreakProvided_ShouldReturnIssue()
    {
        var shift = CreateShift(durationHours: 6m, hasMealBreak: false);

        var issues = _rule.Evaluate(new[] { shift }, _validationId);

        issues.Should().HaveCount(1);
        issues[0].Severity.Should().Be(IssueSeverity.Error);
        issues[0].ExpectedValue.Should().Be(30);
        issues[0].ActualValue.Should().Be(0);
    }

    [Fact]
    public void Evaluate_WhenMealBreakTooShort_ShouldReturnIssue()
    {
        var shift = CreateShift(durationHours: 7m, hasMealBreak: true, mealBreakMinutes: 20);

        var issues = _rule.Evaluate(new[] { shift }, _validationId);

        issues.Should().HaveCount(1);
        issues[0].Severity.Should().Be(IssueSeverity.Error);
        issues[0].ExpectedValue.Should().Be(30);
        issues[0].ActualValue.Should().Be(20);
    }

    [Fact]
    public void Evaluate_WhenMealBreakMeetsRequirement_ShouldReturnNoIssues()
    {
        var shift = CreateShift(durationHours: 8m, hasMealBreak: true, mealBreakMinutes: 30);

        var issues = _rule.Evaluate(new[] { shift }, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenShiftExactly5Hours_ShouldReturnNoIssues()
    {
        // Exactly 5 hours - no break required (threshold is >5)
        var shift = CreateShift(durationHours: 5m, hasMealBreak: false);

        var issues = _rule.Evaluate(new[] { shift }, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenShiftJustOver5Hours_ShouldRequireBreak()
    {
        // 5.01 hours - just over threshold, needs 30 min break
        var shift = CreateShift(durationHours: 5.01m, hasMealBreak: false);

        var issues = _rule.Evaluate(new[] { shift }, _validationId);

        issues.Should().HaveCount(1);
        issues[0].ExpectedValue.Should().Be(30);
    }

    [Fact]
    public void Evaluate_WhenShiftOver9Hours_ShouldRequire60MinBreak()
    {
        // 9.5 hours - needs 60 min break
        var shift = CreateShift(durationHours: 9.5m, hasMealBreak: true, mealBreakMinutes: 45);

        var issues = _rule.Evaluate(new[] { shift }, _validationId);

        issues.Should().HaveCount(1);
        issues[0].ExpectedValue.Should().Be(60);
        issues[0].ActualValue.Should().Be(45);
    }

    [Fact]
    public void Evaluate_WhenShiftOver10Hours_ShouldRequire60MinBreak()
    {
        // 11 hours - needs 60 min break
        var shift = CreateShift(durationHours: 11m, hasMealBreak: true, mealBreakMinutes: 60);

        var issues = _rule.Evaluate(new[] { shift }, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenShiftAt9Hours_ShouldRequire30MinBreak()
    {
        // Exactly 9 hours - still in 8-9 range, needs 30 min
        var shift = CreateShift(durationHours: 9m, hasMealBreak: true, mealBreakMinutes: 30);

        var issues = _rule.Evaluate(new[] { shift }, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenMultipleShifts_ShouldEvaluateEach()
    {
        var validShift = CreateShift(durationHours: 4m, hasMealBreak: false);
        var invalidShift = CreateShift(durationHours: 6m, hasMealBreak: false);

        var issues = _rule.Evaluate(new[] { validShift, invalidShift }, _validationId);

        issues.Should().HaveCount(1);
    }

    private static Shift CreateShift(
        decimal durationHours,
        bool hasMealBreak,
        int? mealBreakMinutes = null
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
            EmploymentType = EmploymentType.FullTime,
            StartDate = new DateTime(2025, 1, 1),
            AwardType = AwardType.GeneralRetailIndustryAward2020,
            AwardLevelNumber = 1,
        };

        var startTime = new TimeSpan(9, 0, 0);
        var endTime = startTime.Add(TimeSpan.FromHours((double)durationHours));

        return new Shift
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            RosterId = Guid.NewGuid(),
            EmployeeId = employeeId,
            Employee = employee,
            Date = new DateTime(2026, 1, 6),
            StartTime = startTime,
            EndTime = endTime,
            HasMealBreak = hasMealBreak,
            MealBreakDuration = mealBreakMinutes,
        };
    }
}
