using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Employees.Entities;
using FairWorkly.Domain.Roster.Entities;
using FairWorkly.Domain.Roster.Enums;
using FairWorkly.Domain.Roster.Rules;
using FluentAssertions;

namespace FairWorkly.UnitTests.Unit;

public class DataQualityRuleTests
{
    private readonly DataQualityRule _rule = new();
    private readonly Guid _validationId = Guid.NewGuid();

    [Fact]
    public void CheckType_ShouldBeDataQuality()
    {
        _rule.CheckType.Should().Be(RosterCheckType.DataQuality);
    }

    [Fact]
    public void Evaluate_WhenBreakMinutesExceedShiftDurationMinutes_ShouldReturnIssue()
    {
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "Employee",
            JobTitle = "Staff",
            EmploymentType = EmploymentType.FullTime,
            StartDate = new DateTime(2025, 1, 1),
            AwardType = AwardType.GeneralRetailIndustryAward2020,
            AwardLevelNumber = 1,
        };

        var shift = new Shift
        {
            Id = Guid.NewGuid(),
            OrganizationId = employee.OrganizationId,
            RosterId = Guid.NewGuid(),
            EmployeeId = employee.Id,
            Employee = employee,
            Date = new DateTime(2026, 1, 5),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0), // 60 minutes
            HasMealBreak = true,
            MealBreakDuration = 90, // 90 minutes break
        };

        var issues = _rule.Evaluate(new[] { shift }, _validationId);

        issues.Should().HaveCount(1);
        issues[0].CheckType.Should().Be(RosterCheckType.DataQuality);
        issues[0].Severity.Should().Be(IssueSeverity.Warning);
        issues[0].ExpectedValue.Should().Be(60m);
        issues[0].ActualValue.Should().Be(90m);
    }

    [Fact]
    public void Evaluate_WhenBreakMinutesWithinShiftDurationMinutes_ShouldReturnNoIssues()
    {
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "Employee",
            JobTitle = "Staff",
            EmploymentType = EmploymentType.FullTime,
            StartDate = new DateTime(2025, 1, 1),
            AwardType = AwardType.GeneralRetailIndustryAward2020,
            AwardLevelNumber = 1,
        };

        var shift = new Shift
        {
            Id = Guid.NewGuid(),
            OrganizationId = employee.OrganizationId,
            RosterId = Guid.NewGuid(),
            EmployeeId = employee.Id,
            Employee = employee,
            Date = new DateTime(2026, 1, 5),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(17, 0, 0), // 480 minutes
            HasMealBreak = true,
            MealBreakDuration = 30,
            HasRestBreaks = true,
            RestBreaksDuration = 20,
        };

        var issues = _rule.Evaluate(new[] { shift }, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenEmployeeIsNull_ShouldReturnDataQualityError()
    {
        var orgId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();

        var shift = new Shift
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId,
            RosterId = Guid.NewGuid(),
            EmployeeId = employeeId,
            Employee = null!, // Missing Employee navigation property
            Date = new DateTime(2026, 1, 5),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(17, 0, 0),
        };

        var issues = _rule.Evaluate(new[] { shift }, _validationId);

        issues.Should().HaveCount(1);
        issues[0].CheckType.Should().Be(RosterCheckType.DataQuality);
        issues[0].Severity.Should().Be(IssueSeverity.Error);
        issues[0].EmployeeId.Should().Be(employeeId);
        issues[0].Description.Should().Contain("Employee data not loaded");
    }

    [Fact]
    public void Evaluate_WhenMultipleShiftsSameEmployeeWithNullEmployee_ShouldReturnSingleIssue()
    {
        var orgId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();

        var shifts = new[]
        {
            new Shift
            {
                Id = Guid.NewGuid(),
                OrganizationId = orgId,
                RosterId = Guid.NewGuid(),
                EmployeeId = employeeId,
                Employee = null!,
                Date = new DateTime(2026, 1, 5),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(17, 0, 0),
            },
            new Shift
            {
                Id = Guid.NewGuid(),
                OrganizationId = orgId,
                RosterId = Guid.NewGuid(),
                EmployeeId = employeeId,
                Employee = null!,
                Date = new DateTime(2026, 1, 6),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(17, 0, 0),
            },
        };

        var issues = _rule.Evaluate(shifts, _validationId);

        // Should only report once per employee, not per shift
        issues.Should().HaveCount(1);
        issues[0].EmployeeId.Should().Be(employeeId);
    }
}
