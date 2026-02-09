using FairWorkly.Domain.Roster.Rules;
using FairWorkly.Domain.Roster.Parameters;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Employees.Entities;
using FairWorkly.Domain.Roster.Entities;
using FairWorkly.Domain.Roster.Enums;
using FluentAssertions;

namespace FairWorkly.UnitTests.Unit;

public class WeeklyHoursLimitRuleTests
{
    private static readonly IRosterRuleParametersProvider ParametersProvider =
        new AwardRosterRuleParametersProvider();

    private readonly WeeklyHoursLimitRule _rule = new(ParametersProvider);
    private readonly Guid _validationId = Guid.NewGuid();

    [Fact]
    public void CheckType_ShouldBeWeeklyHoursLimit()
    {
        _rule.CheckType.Should().Be(RosterCheckType.WeeklyHoursLimit);
    }

    // ==================== Full-Time Tests ====================

    [Fact]
    public void Evaluate_WhenFullTimeAtWeeklyLimit_ShouldReturnNoIssues()
    {
        // 38 hours exactly - no issue
        var shifts = CreateWeeklyShifts(EmploymentType.FullTime, dailyHours: 9.5m, days: 4);

        var issues = _rule.Evaluate(shifts, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenFullTimeOverWeeklyLimit_ShouldReturnIssue()
    {
        // 40 hours > 38 - should trigger info
        var shifts = CreateWeeklyShifts(EmploymentType.FullTime, dailyHours: 8m, days: 5);

        var issues = _rule.Evaluate(shifts, _validationId);

        issues.Should().HaveCount(1);
        issues[0].Severity.Should().Be(IssueSeverity.Info);
        issues[0].ExpectedValue.Should().Be(38m);
        issues[0].ActualValue.Should().Be(40m);
    }

    // ==================== Part-Time Tests ====================

    [Fact]
    public void Evaluate_WhenPartTimeExceedsGuaranteedHours_ShouldReturnIssue()
    {
        // Part-time with 20 guaranteed hours, rostered for 25 - should trigger warning
        var shifts = CreateWeeklyShiftsWithGuaranteedHours(
            EmploymentType.PartTime,
            guaranteedHours: 20,
            dailyHours: 5m,
            days: 5  // 25 hours total
        );

        var issues = _rule.Evaluate(shifts, _validationId);

        issues.Should().HaveCount(1);
        issues[0].Severity.Should().Be(IssueSeverity.Warning);
        issues[0].ExpectedValue.Should().Be(20m);
        issues[0].ActualValue.Should().Be(25m);
        issues[0].Description.Should().Contain("guaranteed");
    }

    [Fact]
    public void Evaluate_WhenPartTimeAtGuaranteedHours_ShouldReturnNoIssues()
    {
        // Part-time with 20 guaranteed hours, rostered for exactly 20 - no issue
        var shifts = CreateWeeklyShiftsWithGuaranteedHours(
            EmploymentType.PartTime,
            guaranteedHours: 20,
            dailyHours: 5m,
            days: 4  // 20 hours total
        );

        var issues = _rule.Evaluate(shifts, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenPartTimeBelowGuaranteedHours_ShouldReturnNoIssues()
    {
        // Part-time with 20 guaranteed hours, rostered for 15 - no issue
        var shifts = CreateWeeklyShiftsWithGuaranteedHours(
            EmploymentType.PartTime,
            guaranteedHours: 20,
            dailyHours: 5m,
            days: 3  // 15 hours total
        );

        var issues = _rule.Evaluate(shifts, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenPartTimeWithoutGuaranteedHours_ShouldReturnDataQualityWarning()
    {
        // Part-time without GuaranteedHours set - should return DataQuality warning
        var shifts = CreateWeeklyShifts(EmploymentType.PartTime, dailyHours: 8m, days: 5);
        // Note: CreateWeeklyShifts doesn't set GuaranteedHours

        var issues = _rule.Evaluate(shifts, _validationId);

        issues.Should().HaveCount(1);
        issues[0].CheckType.Should().Be(RosterCheckType.DataQuality);
        issues[0].Severity.Should().Be(IssueSeverity.Warning);
        issues[0].Description.Should().Contain("GuaranteedHours");
    }

    // ==================== Casual Tests ====================

    [Fact]
    public void Evaluate_WhenCasualOverWeeklyLimit_ShouldReturnNoIssues()
    {
        // Casual employees have no weekly hours limit under Retail Award
        var shifts = CreateWeeklyShifts(EmploymentType.Casual, dailyHours: 8m, days: 5);

        var issues = _rule.Evaluate(shifts, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenCasualAtWeeklyLimit_ShouldReturnNoIssues()
    {
        // Casual employees have no weekly hours limit under Retail Award
        var shifts = CreateWeeklyShifts(EmploymentType.Casual, dailyHours: 9.5m, days: 4);

        var issues = _rule.Evaluate(shifts, _validationId);

        issues.Should().BeEmpty();
    }

    // ==================== Multi-Employee / Multi-Week Tests ====================

    [Fact]
    public void Evaluate_WhenMultipleEmployees_ShouldEvaluateSeparately()
    {
        var employee1Shifts = CreateWeeklyShiftsForEmployee(
            Guid.NewGuid(), EmploymentType.FullTime, dailyHours: 10m, days: 4);
        var employee2Shifts = CreateWeeklyShiftsForEmployee(
            Guid.NewGuid(), EmploymentType.FullTime, dailyHours: 7m, days: 5);

        var allShifts = employee1Shifts.Concat(employee2Shifts).ToList();
        var issues = _rule.Evaluate(allShifts, _validationId);

        // Employee 1: 40 hours (issue), Employee 2: 35 hours (no issue)
        issues.Should().HaveCount(1);
        issues[0].ActualValue.Should().Be(40m);
    }

    [Fact]
    public void Evaluate_WhenShiftsSpanTwoWeeks_ShouldEvaluateEachWeekSeparately()
    {
        var organizationId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var employee = CreateEmployee(organizationId, employeeId, EmploymentType.FullTime);

        var shifts = new List<Shift>();

        // Week 1: 40 hours (issue)
        for (var i = 0; i < 5; i++)
        {
            shifts.Add(CreateShiftForEmployee(employee, new DateTime(2026, 1, 5).AddDays(i), 8m));
        }

        // Week 2: 30 hours (no issue)
        for (var i = 0; i < 5; i++)
        {
            shifts.Add(CreateShiftForEmployee(employee, new DateTime(2026, 1, 12).AddDays(i), 6m));
        }

        var issues = _rule.Evaluate(shifts, _validationId);

        issues.Should().HaveCount(1);
        issues[0].ActualValue.Should().Be(40m);
    }

    [Fact]
    public void Evaluate_WhenNoEmployee_ShouldSkip()
    {
        var shift = new Shift
        {
            Id = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            RosterId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            Employee = null!,
            Date = new DateTime(2026, 1, 5),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(17, 0, 0),
            HasMealBreak = false,
        };

        var issues = _rule.Evaluate(new[] { shift }, _validationId);

        issues.Should().BeEmpty();
    }

    // ==================== Helper Methods ====================

    private static List<Shift> CreateWeeklyShifts(
        EmploymentType employmentType,
        decimal dailyHours,
        int days
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
            // GuaranteedHours not set for this helper
        };

        return CreateShiftsForEmployee(employee, dailyHours, days);
    }

    private static List<Shift> CreateWeeklyShiftsWithGuaranteedHours(
        EmploymentType employmentType,
        int guaranteedHours,
        decimal dailyHours,
        int days
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
            GuaranteedHours = guaranteedHours,
        };

        return CreateShiftsForEmployee(employee, dailyHours, days);
    }

    private static List<Shift> CreateWeeklyShiftsForEmployee(
        Guid employeeId,
        EmploymentType employmentType,
        decimal dailyHours,
        int days
    )
    {
        var organizationId = Guid.NewGuid();
        var employee = new Employee
        {
            Id = employeeId,
            OrganizationId = organizationId,
            FirstName = "Test",
            LastName = "Employee",
            Email = $"test{employeeId}@example.com",
            JobTitle = "Staff",
            EmploymentType = employmentType,
            StartDate = new DateTime(2025, 1, 1),
            AwardType = AwardType.GeneralRetailIndustryAward2020,
            AwardLevelNumber = 1,
        };

        return CreateShiftsForEmployee(employee, dailyHours, days);
    }

    private static List<Shift> CreateShiftsForEmployee(Employee employee, decimal dailyHours, int days)
    {
        var shifts = new List<Shift>();
        var startDate = new DateTime(2026, 1, 5); // Monday
        var startTime = new TimeSpan(9, 0, 0);

        for (var i = 0; i < days; i++)
        {
            var date = startDate.AddDays(i);
            var endTime = startTime.Add(TimeSpan.FromHours((double)dailyHours));

            shifts.Add(
                new Shift
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = employee.OrganizationId,
                    RosterId = Guid.NewGuid(),
                    EmployeeId = employee.Id,
                    Employee = employee,
                    Date = date,
                    StartTime = startTime,
                    EndTime = endTime,
                    HasMealBreak = false,
                }
            );
        }

        return shifts;
    }

    private static Employee CreateEmployee(Guid organizationId, Guid employeeId, EmploymentType employmentType)
    {
        return new Employee
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
    }

    private static Shift CreateShiftForEmployee(Employee employee, DateTime date, decimal hours)
    {
        var startTime = new TimeSpan(9, 0, 0);
        return new Shift
        {
            Id = Guid.NewGuid(),
            OrganizationId = employee.OrganizationId,
            RosterId = Guid.NewGuid(),
            EmployeeId = employee.Id,
            Employee = employee,
            Date = date,
            StartTime = startTime,
            EndTime = startTime.Add(TimeSpan.FromHours((double)hours)),
            HasMealBreak = false,
        };
    }
}
