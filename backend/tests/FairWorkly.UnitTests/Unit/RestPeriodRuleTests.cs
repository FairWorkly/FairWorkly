using FairWorkly.Domain.Roster.Rules;
using FairWorkly.Domain.Roster.Parameters;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Employees.Entities;
using FairWorkly.Domain.Roster.Entities;
using FairWorkly.Domain.Roster.Enums;
using FluentAssertions;

namespace FairWorkly.UnitTests.Unit;

public class RestPeriodRuleTests
{
    private static readonly IRosterRuleParametersProvider ParametersProvider =
        new AwardRosterRuleParametersProvider();

    private readonly RestPeriodRule _rule = new(ParametersProvider);
    private readonly Guid _validationId = Guid.NewGuid();

    [Fact]
    public void CheckType_ShouldBeRestPeriodBetweenShifts()
    {
        _rule.CheckType.Should().Be(RosterCheckType.RestPeriodBetweenShifts);
    }

    [Fact]
    public void Evaluate_WhenRestPeriodAtOrAboveMinimum_ShouldReturnNoIssues()
    {
        var shifts = CreateTwoShifts(restHours: 13);

        var issues = _rule.Evaluate(shifts, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenRestPeriodBelowMinimum_ShouldReturnIssue()
    {
        var shifts = CreateTwoShifts(restHours: 9);

        var issues = _rule.Evaluate(shifts, _validationId);

        issues.Should().HaveCount(1);
        issues[0].Severity.Should().Be(IssueSeverity.Error);
        issues[0].ExpectedValue.Should().Be(10);
        issues[0].AffectedShiftsCount.Should().Be(2);
    }

    [Fact]
    public void Evaluate_WhenRestPeriodExactly12Hours_ShouldReturnNoIssues()
    {
        var shifts = CreateTwoShifts(restHours: 12);

        var issues = _rule.Evaluate(shifts, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenRestPeriodJustBelow12Hours_ShouldReturnIssue()
    {
        var shifts = CreateTwoShiftsWithMinutes(restHours: 11, restMinutes: 59);

        var issues = _rule.Evaluate(shifts, _validationId);

        issues.Should().HaveCount(1);
        issues[0].Severity.Should().Be(IssueSeverity.Warning);
        issues[0].ExpectedValue.Should().Be(12);
    }

    [Fact]
    public void Evaluate_WhenMultipleEmployees_ShouldEvaluateSeparately()
    {
        var employee1Shifts = CreateTwoShiftsForEmployee(Guid.NewGuid(), restHours: 8);
        var employee2Shifts = CreateTwoShiftsForEmployee(Guid.NewGuid(), restHours: 14);

        var allShifts = employee1Shifts.Concat(employee2Shifts).ToList();
        var issues = _rule.Evaluate(allShifts, _validationId);

        // Only employee 1 should have an issue
        issues.Should().HaveCount(1);
    }

    [Fact]
    public void Evaluate_WhenThreeShiftsWithOneViolation_ShouldReturnOneIssue()
    {
        var organizationId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var employee = CreateEmployee(organizationId, employeeId);

        var shifts = new List<Shift>
        {
            CreateShiftForEmployee(employee, new DateTime(2026, 1, 6), 8, 16),  // Day 1: 8am-4pm
            CreateShiftForEmployee(employee, new DateTime(2026, 1, 7), 9, 17),  // Day 2: 9am-5pm (17hr rest - OK)
            CreateShiftForEmployee(employee, new DateTime(2026, 1, 7), 22, 6),  // Day 2 night: 10pm-6am (5hr rest - ISSUE)
        };

        var issues = _rule.Evaluate(shifts, _validationId);

        issues.Should().HaveCount(1);
    }

    [Fact]
    public void Evaluate_WhenSingleShift_ShouldReturnNoIssues()
    {
        var organizationId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var employee = CreateEmployee(organizationId, employeeId);

        var shift = CreateShiftForEmployee(employee, new DateTime(2026, 1, 6), 9, 17);

        var issues = _rule.Evaluate(new[] { shift }, _validationId);

        issues.Should().BeEmpty();
    }

    private static List<Shift> CreateTwoShifts(int restHours)
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

        var firstShiftDate = new DateTime(2026, 1, 6);
        var firstShiftStart = new TimeSpan(8, 0, 0);
        var firstShiftEnd = new TimeSpan(16, 0, 0);

        var firstShiftEndDateTime = firstShiftDate.Add(firstShiftEnd);
        var secondShiftStartDateTime = firstShiftEndDateTime.AddHours(restHours);

        return new List<Shift>
        {
            new()
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                RosterId = Guid.NewGuid(),
                EmployeeId = employeeId,
                Employee = employee,
                Date = firstShiftDate,
                StartTime = firstShiftStart,
                EndTime = firstShiftEnd,
                HasMealBreak = false,
            },
            new()
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                RosterId = Guid.NewGuid(),
                EmployeeId = employeeId,
                Employee = employee,
                Date = secondShiftStartDateTime.Date,
                StartTime = secondShiftStartDateTime.TimeOfDay,
                EndTime = secondShiftStartDateTime.TimeOfDay.Add(TimeSpan.FromHours(8)),
                HasMealBreak = false,
            },
        };
    }

    private static List<Shift> CreateTwoShiftsWithMinutes(int restHours, int restMinutes)
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

        var firstShiftDate = new DateTime(2026, 1, 6);
        var firstShiftStart = new TimeSpan(8, 0, 0);
        var firstShiftEnd = new TimeSpan(16, 0, 0);

        var firstShiftEndDateTime = firstShiftDate.Add(firstShiftEnd);
        var secondShiftStartDateTime = firstShiftEndDateTime.AddHours(restHours).AddMinutes(restMinutes);

        return new List<Shift>
        {
            new()
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                RosterId = Guid.NewGuid(),
                EmployeeId = employeeId,
                Employee = employee,
                Date = firstShiftDate,
                StartTime = firstShiftStart,
                EndTime = firstShiftEnd,
                HasMealBreak = false,
            },
            new()
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                RosterId = Guid.NewGuid(),
                EmployeeId = employeeId,
                Employee = employee,
                Date = secondShiftStartDateTime.Date,
                StartTime = secondShiftStartDateTime.TimeOfDay,
                EndTime = secondShiftStartDateTime.TimeOfDay.Add(TimeSpan.FromHours(8)),
                HasMealBreak = false,
            },
        };
    }

    private static List<Shift> CreateTwoShiftsForEmployee(Guid employeeId, int restHours)
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
            EmploymentType = EmploymentType.FullTime,
            StartDate = new DateTime(2025, 1, 1),
            AwardType = AwardType.GeneralRetailIndustryAward2020,
            AwardLevelNumber = 1,
        };

        var firstShiftDate = new DateTime(2026, 1, 6);
        var firstShiftStart = new TimeSpan(8, 0, 0);
        var firstShiftEnd = new TimeSpan(16, 0, 0);

        var firstShiftEndDateTime = firstShiftDate.Add(firstShiftEnd);
        var secondShiftStartDateTime = firstShiftEndDateTime.AddHours(restHours);

        return new List<Shift>
        {
            new()
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                RosterId = Guid.NewGuid(),
                EmployeeId = employeeId,
                Employee = employee,
                Date = firstShiftDate,
                StartTime = firstShiftStart,
                EndTime = firstShiftEnd,
                HasMealBreak = false,
            },
            new()
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                RosterId = Guid.NewGuid(),
                EmployeeId = employeeId,
                Employee = employee,
                Date = secondShiftStartDateTime.Date,
                StartTime = secondShiftStartDateTime.TimeOfDay,
                EndTime = secondShiftStartDateTime.TimeOfDay.Add(TimeSpan.FromHours(8)),
                HasMealBreak = false,
            },
        };
    }

    private static Employee CreateEmployee(Guid organizationId, Guid employeeId)
    {
        return new Employee
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
    }

    private static Shift CreateShiftForEmployee(Employee employee, DateTime date, int startHour, int endHour)
    {
        return new Shift
        {
            Id = Guid.NewGuid(),
            OrganizationId = employee.OrganizationId,
            RosterId = Guid.NewGuid(),
            EmployeeId = employee.Id,
            Employee = employee,
            Date = date,
            StartTime = new TimeSpan(startHour, 0, 0),
            EndTime = new TimeSpan(endHour, 0, 0),
            HasMealBreak = false,
        };
    }
}
