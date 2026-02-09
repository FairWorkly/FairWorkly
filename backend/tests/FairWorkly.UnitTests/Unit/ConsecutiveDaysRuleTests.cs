using FairWorkly.Domain.Roster.Rules;
using FairWorkly.Domain.Roster.Parameters;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Employees.Entities;
using FairWorkly.Domain.Roster.Entities;
using FairWorkly.Domain.Roster.Enums;
using FluentAssertions;

namespace FairWorkly.UnitTests.Unit;

public class ConsecutiveDaysRuleTests
{
    private static readonly IRosterRuleParametersProvider ParametersProvider =
        new AwardRosterRuleParametersProvider();

    private readonly ConsecutiveDaysRule _rule = new(ParametersProvider);
    private readonly Guid _validationId = Guid.NewGuid();

    [Fact]
    public void CheckType_ShouldBeMaximumConsecutiveDays()
    {
        _rule.CheckType.Should().Be(RosterCheckType.MaximumConsecutiveDays);
    }

    [Fact]
    public void Evaluate_WhenSixConsecutiveDays_ShouldReturnNoIssues()
    {
        var employee = CreateEmployee(Guid.NewGuid(), Guid.NewGuid());
        var shifts = CreateConsecutiveShifts(employee, 6);

        var issues = _rule.Evaluate(shifts, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenSevenConsecutiveDays_ShouldReturnIssue()
    {
        var employee = CreateEmployee(Guid.NewGuid(), Guid.NewGuid());
        var shifts = CreateConsecutiveShifts(employee, 7);

        var issues = _rule.Evaluate(shifts, _validationId);

        issues.Should().HaveCount(1);
        issues[0].Severity.Should().Be(IssueSeverity.Warning);
        issues[0].ExpectedValue.Should().Be(6);
        issues[0].ActualValue.Should().Be(7);
        issues[0].AffectedShiftsCount.Should().Be(7);
    }

    [Fact]
    public void Evaluate_WhenBreakInSequence_ShouldReturnNoIssues()
    {
        var organizationId = Guid.NewGuid();
        var employee = CreateEmployee(organizationId, Guid.NewGuid());
        var shifts = CreateConsecutiveShifts(employee, 3, startDate: new DateTime(2026, 1, 1));
        shifts.AddRange(CreateConsecutiveShifts(employee, 3, startDate: new DateTime(2026, 1, 10)));

        var issues = _rule.Evaluate(shifts, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenEightConsecutiveDays_ShouldReturnIssue()
    {
        var employee = CreateEmployee(Guid.NewGuid(), Guid.NewGuid());
        var shifts = CreateConsecutiveShifts(employee, 8);

        var issues = _rule.Evaluate(shifts, _validationId);

        issues.Should().HaveCount(1);
        issues[0].ActualValue.Should().Be(8);
        issues[0].AffectedShiftsCount.Should().Be(8);
    }

    [Fact]
    public void Evaluate_WhenMultipleEmployees_ShouldEvaluateSeparately()
    {
        var employee1 = CreateEmployee(Guid.NewGuid(), Guid.NewGuid());
        var employee2 = CreateEmployee(Guid.NewGuid(), Guid.NewGuid());

        var employee1Shifts = CreateConsecutiveShifts(employee1, 7); // Issue
        var employee2Shifts = CreateConsecutiveShifts(employee2, 5); // No issue

        var allShifts = employee1Shifts.Concat(employee2Shifts).ToList();
        var issues = _rule.Evaluate(allShifts, _validationId);

        issues.Should().HaveCount(1);
        issues[0].EmployeeId.Should().Be(employee1.Id);
    }

    [Fact]
    public void Evaluate_WhenTwoSeparateViolations_ShouldReturnTwoIssues()
    {
        var employee = CreateEmployee(Guid.NewGuid(), Guid.NewGuid());

        // First violation: 7 consecutive days
        var shifts1 = CreateConsecutiveShifts(employee, 7, startDate: new DateTime(2026, 1, 1));
        // Gap
        // Second violation: 8 consecutive days
        var shifts2 = CreateConsecutiveShifts(employee, 8, startDate: new DateTime(2026, 1, 15));

        var allShifts = shifts1.Concat(shifts2).ToList();
        var issues = _rule.Evaluate(allShifts, _validationId);

        issues.Should().HaveCount(2);
    }

    [Fact]
    public void Evaluate_WhenSingleShift_ShouldReturnNoIssues()
    {
        var employee = CreateEmployee(Guid.NewGuid(), Guid.NewGuid());
        var shifts = CreateConsecutiveShifts(employee, 1);

        var issues = _rule.Evaluate(shifts, _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenEmptyShifts_ShouldReturnNoIssues()
    {
        var issues = _rule.Evaluate(new List<Shift>(), _validationId);

        issues.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_WhenMultipleShiftsSameDay_ShouldCountAsOneDay()
    {
        var employee = CreateEmployee(Guid.NewGuid(), Guid.NewGuid());
        var shifts = new List<Shift>();

        // 7 days but with 2 shifts on some days = should still be 7 consecutive days (issue)
        for (var i = 0; i < 7; i++)
        {
            var date = new DateTime(2026, 1, 1).AddDays(i);
            shifts.Add(CreateShiftForEmployee(employee, date, 9, 13));  // Morning shift
            shifts.Add(CreateShiftForEmployee(employee, date, 14, 18)); // Afternoon shift
        }

        var issues = _rule.Evaluate(shifts, _validationId);

        issues.Should().HaveCount(1);
        issues[0].ActualValue.Should().Be(7);
    }

    private static List<Shift> CreateConsecutiveShifts(
        Employee employee,
        int days,
        DateTime? startDate = null
    )
    {
        var organizationId = employee.OrganizationId;

        var shifts = new List<Shift>();
        var baseDate = startDate ?? new DateTime(2026, 1, 1);
        var startTime = new TimeSpan(9, 0, 0);

        for (var i = 0; i < days; i++)
        {
            var date = baseDate.AddDays(i);
            shifts.Add(
                new Shift
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = organizationId,
                    RosterId = Guid.NewGuid(),
                    EmployeeId = employee.Id,
                    Employee = employee,
                    Date = date,
                    StartTime = startTime,
                    EndTime = startTime.Add(TimeSpan.FromHours(8)),
                    HasMealBreak = false,
                }
            );
        }

        return shifts;
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
