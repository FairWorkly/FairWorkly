using FairWorkly.Application.Roster.Features.ValidateRoster;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Employees.Entities;
using FairWorkly.Domain.Roster.Entities;
using FairWorkly.Domain.Roster.Enums;
using FairWorkly.Domain.Roster.ValueObjects;
using FluentAssertions;
using Xunit;
using RosterEntity = FairWorkly.Domain.Roster.Entities.Roster;

namespace FairWorkly.UnitTests.Unit;

public class ValidationResponseBuilderTests
{
    private readonly Guid _orgId = Guid.NewGuid();
    private readonly Guid _rosterId = Guid.NewGuid();
    private readonly Guid _validationId = Guid.NewGuid();
    private readonly Guid _employee1Id = Guid.NewGuid();
    private readonly Guid _employee2Id = Guid.NewGuid();

    private Employee CreateEmployee(Guid id, string firstName, string lastName, string empNumber)
    {
        return new Employee
        {
            Id = id,
            OrganizationId = _orgId,
            FirstName = firstName,
            LastName = lastName,
            EmployeeNumber = empNumber,
            JobTitle = "Staff",
            AwardType = AwardType.GeneralRetailIndustryAward2020,
            AwardLevelNumber = 1,
            EmploymentType = EmploymentType.FullTime,
            StartDate = DateTime.SpecifyKind(DateTime.UtcNow.AddYears(-1), DateTimeKind.Utc),
            IsActive = true,
        };
    }

    private RosterEntity CreateRoster(params (Guid employeeId, Employee employee)[] employees)
    {
        var weekStart = new DateTime(2026, 2, 2, 0, 0, 0, DateTimeKind.Utc);
        var roster = new RosterEntity
        {
            Id = _rosterId,
            OrganizationId = _orgId,
            WeekStartDate = weekStart,
            WeekEndDate = weekStart.AddDays(6),
            WeekNumber = 6,
            Year = 2026,
            TotalEmployees = employees.Length,
        };

        foreach (var (employeeId, employee) in employees)
        {
            roster.Shifts.Add(
                new Shift
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = _orgId,
                    RosterId = _rosterId,
                    EmployeeId = employeeId,
                    Employee = employee,
                    Date = weekStart,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    HasMealBreak = true,
                    MealBreakDuration = 30,
                }
            );
        }

        return roster;
    }

    [Fact]
    public void Build_MapsValidationFieldsCorrectly()
    {
        // Arrange
        var emp = CreateEmployee(_employee1Id, "Alice", "Smith", "EMP001");
        var roster = CreateRoster((_employee1Id, emp));
        var completedAt = DateTimeOffset.UtcNow;

        var validation = new RosterValidation
        {
            Id = _validationId,
            OrganizationId = _orgId,
            RosterId = _rosterId,
            Status = ValidationStatus.Passed,
            TotalShifts = 5,
            PassedShifts = 5,
            FailedShifts = 0,
            TotalIssuesCount = 0,
            CriticalIssuesCount = 0,
            AffectedEmployees = 0,
            WeekStartDate = roster.WeekStartDate,
            WeekEndDate = roster.WeekEndDate,
            CompletedAt = completedAt,
        };

        // Act
        var result = ValidationResponseBuilder.Build(roster, validation, new List<RosterIssue>());

        // Assert
        result.ValidationId.Should().Be(_validationId);
        result.Status.Should().Be(ValidationStatus.Passed);
        result.TotalShifts.Should().Be(5);
        result.PassedShifts.Should().Be(5);
        result.FailedShifts.Should().Be(0);
        result.TotalIssues.Should().Be(0);
        result.CriticalIssues.Should().Be(0);
        result.AffectedEmployees.Should().Be(0);
        result.TotalEmployees.Should().Be(1);
        result.WeekStartDate.Should().Be(roster.WeekStartDate);
        result.WeekEndDate.Should().Be(roster.WeekEndDate);
        result.ValidatedAt.Should().Be(completedAt);
        result.FailureType.Should().BeNull();
        result.Retriable.Should().BeNull();
        result.Issues.Should().BeEmpty();
    }

    [Fact]
    public void Build_MapsIssuesWithEmployeeNames()
    {
        // Arrange
        var emp1 = CreateEmployee(_employee1Id, "Alice", "Smith", "EMP001");
        var emp2 = CreateEmployee(_employee2Id, "Bob", "Jones", "EMP002");
        var roster = CreateRoster((_employee1Id, emp1), (_employee2Id, emp2));

        var validation = new RosterValidation
        {
            Id = _validationId,
            OrganizationId = _orgId,
            RosterId = _rosterId,
            Status = ValidationStatus.Failed,
            TotalShifts = 2,
            PassedShifts = 0,
            FailedShifts = 2,
            TotalIssuesCount = 2,
            CriticalIssuesCount = 2,
            AffectedEmployees = 2,
            WeekStartDate = roster.WeekStartDate,
            WeekEndDate = roster.WeekEndDate,
        };

        var issues = new List<RosterIssue>
        {
            new()
            {
                Id = Guid.NewGuid(),
                OrganizationId = _orgId,
                RosterValidationId = _validationId,
                EmployeeId = _employee1Id,
                ShiftId = roster.Shifts.First().Id,
                CheckType = RosterCheckType.MinimumShiftHours,
                Severity = IssueSeverity.Error,
                Description = "Shift too short",
                ExpectedValue = 3m,
                ActualValue = 2m,
                AffectedDates = AffectedDateSet.FromDates(new[] { new DateOnly(2026, 2, 2) }),
            },
            new()
            {
                Id = Guid.NewGuid(),
                OrganizationId = _orgId,
                RosterValidationId = _validationId,
                EmployeeId = _employee2Id,
                CheckType = RosterCheckType.MealBreak,
                Severity = IssueSeverity.Error,
                Description = "No meal break",
                AffectedDates = AffectedDateSet.Empty,
            },
        };

        // Act
        var result = ValidationResponseBuilder.Build(roster, validation, issues);

        // Assert
        result.Issues.Should().HaveCount(2);

        var issue1 = result.Issues.First(i => i.CheckType == "MinimumShiftHours");
        issue1.EmployeeName.Should().Be("Alice Smith");
        issue1.EmployeeNumber.Should().Be("EMP001");
        issue1.ExpectedValue.Should().Be(3m);
        issue1.ActualValue.Should().Be(2m);
        issue1.AffectedDates.Should().Be("2026-02-02");

        var issue2 = result.Issues.First(i => i.CheckType == "MealBreak");
        issue2.EmployeeName.Should().Be("Bob Jones");
        issue2.EmployeeNumber.Should().Be("EMP002");
        issue2.AffectedDates.Should().BeNull();
        result.FailureType.Should().Be(ValidationFailureType.Compliance);
        result.Retriable.Should().BeFalse();
    }

    [Fact]
    public void Build_UnknownEmployeeId_MapsToNullName()
    {
        // Arrange - issue references an employee not in the roster shifts
        var emp = CreateEmployee(_employee1Id, "Alice", "Smith", "EMP001");
        var roster = CreateRoster((_employee1Id, emp));
        var unknownEmployeeId = Guid.NewGuid();

        var validation = new RosterValidation
        {
            Id = _validationId,
            OrganizationId = _orgId,
            RosterId = _rosterId,
            Status = ValidationStatus.Failed,
            TotalShifts = 1,
            TotalIssuesCount = 1,
            CriticalIssuesCount = 1,
            WeekStartDate = roster.WeekStartDate,
            WeekEndDate = roster.WeekEndDate,
        };

        var issues = new List<RosterIssue>
        {
            new()
            {
                Id = Guid.NewGuid(),
                OrganizationId = _orgId,
                RosterValidationId = _validationId,
                EmployeeId = unknownEmployeeId,
                CheckType = RosterCheckType.DataQuality,
                Severity = IssueSeverity.Warning,
                Description = "Data quality issue",
                AffectedDates = AffectedDateSet.Empty,
            },
        };

        // Act
        var result = ValidationResponseBuilder.Build(roster, validation, issues);

        // Assert
        var issue = result.Issues.Single();
        issue.EmployeeName.Should().BeNull();
        issue.EmployeeNumber.Should().BeNull();
        issue.EmployeeId.Should().Be(unknownEmployeeId);
    }

    [Fact]
    public void Build_MultipleIssuesSameEmployee_AllShareSameName()
    {
        // Arrange
        var emp = CreateEmployee(_employee1Id, "Alice", "Smith", "EMP001");
        var roster = CreateRoster((_employee1Id, emp));

        var validation = new RosterValidation
        {
            Id = _validationId,
            OrganizationId = _orgId,
            RosterId = _rosterId,
            Status = ValidationStatus.Failed,
            TotalShifts = 1,
            TotalIssuesCount = 2,
            CriticalIssuesCount = 2,
            WeekStartDate = roster.WeekStartDate,
            WeekEndDate = roster.WeekEndDate,
        };

        var issues = new List<RosterIssue>
        {
            new()
            {
                Id = Guid.NewGuid(),
                OrganizationId = _orgId,
                RosterValidationId = _validationId,
                EmployeeId = _employee1Id,
                CheckType = RosterCheckType.MinimumShiftHours,
                Severity = IssueSeverity.Error,
                Description = "Shift too short",
                AffectedDates = AffectedDateSet.Empty,
            },
            new()
            {
                Id = Guid.NewGuid(),
                OrganizationId = _orgId,
                RosterValidationId = _validationId,
                EmployeeId = _employee1Id,
                CheckType = RosterCheckType.MealBreak,
                Severity = IssueSeverity.Error,
                Description = "No meal break",
                AffectedDates = AffectedDateSet.Empty,
            },
        };

        // Act
        var result = ValidationResponseBuilder.Build(roster, validation, issues);

        // Assert
        result.Issues.Should().HaveCount(2);
        result
            .Issues.Should()
            .AllSatisfy(i =>
            {
                i.EmployeeName.Should().Be("Alice Smith");
                i.EmployeeNumber.Should().Be("EMP001");
            });
    }

    [Fact]
    public void Build_CheckTypeSerializedAsString()
    {
        // Arrange
        var emp = CreateEmployee(_employee1Id, "Alice", "Smith", "EMP001");
        var roster = CreateRoster((_employee1Id, emp));

        var validation = new RosterValidation
        {
            Id = _validationId,
            OrganizationId = _orgId,
            RosterId = _rosterId,
            Status = ValidationStatus.Failed,
            TotalShifts = 1,
            TotalIssuesCount = 6,
            CriticalIssuesCount = 6,
            WeekStartDate = roster.WeekStartDate,
            WeekEndDate = roster.WeekEndDate,
        };

        var checkTypes = new[]
        {
            RosterCheckType.DataQuality,
            RosterCheckType.MinimumShiftHours,
            RosterCheckType.MealBreak,
            RosterCheckType.RestPeriodBetweenShifts,
            RosterCheckType.WeeklyHoursLimit,
            RosterCheckType.MaximumConsecutiveDays,
        };

        var issues = checkTypes
            .Select(ct => new RosterIssue
            {
                Id = Guid.NewGuid(),
                OrganizationId = _orgId,
                RosterValidationId = _validationId,
                EmployeeId = _employee1Id,
                CheckType = ct,
                Severity = IssueSeverity.Error,
                Description = $"Issue for {ct}",
                AffectedDates = AffectedDateSet.Empty,
            })
            .ToList();

        // Act
        var result = ValidationResponseBuilder.Build(roster, validation, issues);

        // Assert - verify CheckType is serialized as string enum name
        var checkTypeStrings = result.Issues.Select(i => i.CheckType).ToList();
        checkTypeStrings.Should().Contain("DataQuality");
        checkTypeStrings.Should().Contain("MinimumShiftHours");
        checkTypeStrings.Should().Contain("MealBreak");
        checkTypeStrings.Should().Contain("RestPeriodBetweenShifts");
        checkTypeStrings.Should().Contain("WeeklyHoursLimit");
        checkTypeStrings.Should().Contain("MaximumConsecutiveDays");
    }

    [Fact]
    public void Build_ExecutionFailure_IsRetriable()
    {
        // Arrange
        var emp = CreateEmployee(_employee1Id, "Alice", "Smith", "EMP001");
        var roster = CreateRoster((_employee1Id, emp));

        var validation = new RosterValidation
        {
            Id = _validationId,
            OrganizationId = _orgId,
            RosterId = _rosterId,
            Status = ValidationStatus.Failed,
            Notes = "ExecutionFailure: database timeout",
            WeekStartDate = roster.WeekStartDate,
            WeekEndDate = roster.WeekEndDate,
        };

        // Act
        var result = ValidationResponseBuilder.Build(roster, validation, []);

        // Assert
        result.FailureType.Should().Be(ValidationFailureType.Execution);
        result.Retriable.Should().BeTrue();
    }
}
