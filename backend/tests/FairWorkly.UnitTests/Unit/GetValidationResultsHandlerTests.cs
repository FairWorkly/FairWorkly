using FairWorkly.Application.Roster.Features.GetValidationResults;
using FairWorkly.Application.Roster.Features.ValidateRoster;
using FairWorkly.Application.Roster.Interfaces;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Employees.Entities;
using FairWorkly.Domain.Roster.Entities;
using FairWorkly.Domain.Roster.Enums;
using FairWorkly.Domain.Roster.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;
using RosterEntity = FairWorkly.Domain.Roster.Entities.Roster;

namespace FairWorkly.UnitTests.Unit;

public class GetValidationResultsHandlerTests
{
    private readonly Mock<IRosterRepository> _rosterRepoMock = new();
    private readonly Mock<IRosterValidationRepository> _validationRepoMock = new();
    private readonly GetValidationResultsHandler _handler;

    private readonly Guid _orgId = Guid.NewGuid();
    private readonly Guid _rosterId = Guid.NewGuid();
    private readonly Guid _validationId = Guid.NewGuid();
    private readonly Guid _employeeId = Guid.NewGuid();

    public GetValidationResultsHandlerTests()
    {
        _handler = new GetValidationResultsHandler(
            _rosterRepoMock.Object,
            _validationRepoMock.Object
        );
    }

    private RosterEntity CreateTestRoster()
    {
        var employee = new Employee
        {
            Id = _employeeId,
            OrganizationId = _orgId,
            FirstName = "Alice",
            LastName = "Smith",
            EmployeeNumber = "EMP001",
            JobTitle = "Cashier",
            AwardType = AwardType.GeneralRetailIndustryAward2020,
            AwardLevelNumber = 1,
            EmploymentType = EmploymentType.FullTime,
            StartDate = DateTime.SpecifyKind(DateTime.UtcNow.AddYears(-1), DateTimeKind.Utc),
            IsActive = true,
        };

        var weekStart = new DateTime(2026, 2, 2, 0, 0, 0, DateTimeKind.Utc);
        var roster = new RosterEntity
        {
            Id = _rosterId,
            OrganizationId = _orgId,
            WeekStartDate = weekStart,
            WeekEndDate = weekStart.AddDays(6),
            WeekNumber = 6,
            Year = 2026,
            TotalEmployees = 1,
            TotalShifts = 2,
        };

        roster.Shifts.Add(
            new Shift
            {
                Id = Guid.NewGuid(),
                OrganizationId = _orgId,
                RosterId = _rosterId,
                EmployeeId = _employeeId,
                Employee = employee,
                Date = weekStart,
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(17, 0, 0),
                HasMealBreak = true,
                MealBreakDuration = 30,
            }
        );

        return roster;
    }

    private RosterValidation CreateTestValidation(List<RosterIssue>? issues = null)
    {
        var validation = new RosterValidation
        {
            Id = _validationId,
            OrganizationId = _orgId,
            RosterId = _rosterId,
            Status = ValidationStatus.Failed,
            WeekStartDate = new DateTime(2026, 2, 2, 0, 0, 0, DateTimeKind.Utc),
            WeekEndDate = new DateTime(2026, 2, 8, 0, 0, 0, DateTimeKind.Utc),
            TotalShifts = 2,
            PassedShifts = 1,
            FailedShifts = 1,
            TotalIssuesCount = 1,
            CriticalIssuesCount = 1,
            AffectedEmployees = 1,
            StartedAt = DateTimeOffset.UtcNow.AddMinutes(-1),
            CompletedAt = DateTimeOffset.UtcNow,
        };

        if (issues != null)
        {
            foreach (var issue in issues)
                validation.Issues.Add(issue);
        }

        return validation;
    }

    private RosterIssue CreateTestIssue()
    {
        return new RosterIssue
        {
            Id = Guid.NewGuid(),
            OrganizationId = _orgId,
            RosterValidationId = _validationId,
            EmployeeId = _employeeId,
            CheckType = RosterCheckType.MinimumShiftHours,
            Severity = IssueSeverity.Error,
            Description = "Shift only 2 hours, minimum is 3 hours",
            ExpectedValue = 3m,
            ActualValue = 2m,
            AffectedDates = AffectedDateSet.FromDates(new[] { new DateOnly(2026, 2, 2) }),
        };
    }

    [Fact]
    public async Task Handle_NoValidationExists_Returns404()
    {
        // Arrange
        _validationRepoMock
            .Setup(r =>
                r.GetByRosterIdWithIssuesAsync(_rosterId, _orgId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((RosterValidation?)null);

        var query = new GetValidationResultsQuery { RosterId = _rosterId, OrganizationId = _orgId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(404);
        result.Message.Should().Contain("No validation found");
    }

    [Fact]
    public async Task Handle_ValidationExistsButRosterNotFound_Returns404()
    {
        // Arrange
        var validation = CreateTestValidation(new List<RosterIssue> { CreateTestIssue() });

        _validationRepoMock
            .Setup(r =>
                r.GetByRosterIdWithIssuesAsync(_rosterId, _orgId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(validation);

        _rosterRepoMock
            .Setup(r => r.GetByIdWithShiftsAsync(_rosterId, _orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RosterEntity?)null);

        var query = new GetValidationResultsQuery { RosterId = _rosterId, OrganizationId = _orgId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(404);
        result.Message.Should().Contain("Roster not found");
    }

    [Fact]
    public async Task Handle_ValidationAndRosterExist_Returns200WithResults()
    {
        // Arrange
        var issue = CreateTestIssue();
        var validation = CreateTestValidation(new List<RosterIssue> { issue });
        var roster = CreateTestRoster();

        _validationRepoMock
            .Setup(r =>
                r.GetByRosterIdWithIssuesAsync(_rosterId, _orgId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(validation);

        _rosterRepoMock
            .Setup(r => r.GetByIdWithShiftsAsync(_rosterId, _orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roster);

        var query = new GetValidationResultsQuery { RosterId = _rosterId, OrganizationId = _orgId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Code.Should().Be(200);
        result.Value.Should().NotBeNull();

        var response = result.Value!;
        response.ValidationId.Should().Be(_validationId);
        response.Status.Should().Be(ValidationStatus.Failed);
        response.TotalShifts.Should().Be(2);
        response.PassedShifts.Should().Be(1);
        response.FailedShifts.Should().Be(1);
        response.TotalIssues.Should().Be(1);
        response.CriticalIssues.Should().Be(1);
        response.AffectedEmployees.Should().Be(1);
        response.TotalEmployees.Should().Be(1);
        response.Issues.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_MapsEmployeeNamesFromRoster()
    {
        // Arrange
        var issue = CreateTestIssue();
        var validation = CreateTestValidation(new List<RosterIssue> { issue });
        var roster = CreateTestRoster();

        _validationRepoMock
            .Setup(r =>
                r.GetByRosterIdWithIssuesAsync(_rosterId, _orgId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(validation);

        _rosterRepoMock
            .Setup(r => r.GetByIdWithShiftsAsync(_rosterId, _orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roster);

        var query = new GetValidationResultsQuery { RosterId = _rosterId, OrganizationId = _orgId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var issueSummary = result.Value!.Issues.First();
        issueSummary.EmployeeName.Should().Be("Alice Smith");
        issueSummary.EmployeeNumber.Should().Be("EMP001");
        issueSummary.CheckType.Should().Be("MinimumShiftHours");
        issueSummary.Severity.Should().Be(IssueSeverity.Error);
        issueSummary.Description.Should().Be("Shift only 2 hours, minimum is 3 hours");
        issueSummary.ExpectedValue.Should().Be(3m);
        issueSummary.ActualValue.Should().Be(2m);
    }

    [Fact]
    public async Task Handle_PassedValidation_ReturnsCorrectStatus()
    {
        // Arrange
        var validation = new RosterValidation
        {
            Id = _validationId,
            OrganizationId = _orgId,
            RosterId = _rosterId,
            Status = ValidationStatus.Passed,
            WeekStartDate = new DateTime(2026, 2, 2, 0, 0, 0, DateTimeKind.Utc),
            WeekEndDate = new DateTime(2026, 2, 8, 0, 0, 0, DateTimeKind.Utc),
            TotalShifts = 3,
            PassedShifts = 3,
            FailedShifts = 0,
            TotalIssuesCount = 0,
            CriticalIssuesCount = 0,
            AffectedEmployees = 0,
            CompletedAt = DateTimeOffset.UtcNow,
        };

        var roster = CreateTestRoster();

        _validationRepoMock
            .Setup(r =>
                r.GetByRosterIdWithIssuesAsync(_rosterId, _orgId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(validation);

        _rosterRepoMock
            .Setup(r => r.GetByIdWithShiftsAsync(_rosterId, _orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roster);

        var query = new GetValidationResultsQuery { RosterId = _rosterId, OrganizationId = _orgId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(ValidationStatus.Passed);
        result.Value.Issues.Should().BeEmpty();
        result.Value.CriticalIssues.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ExecutionFailedValidation_Returns404ForRetry()
    {
        // Arrange
        var validation = new RosterValidation
        {
            Id = _validationId,
            OrganizationId = _orgId,
            RosterId = _rosterId,
            Status = ValidationStatus.Failed,
            Notes = "ExecutionFailure: transient storage error",
        };

        _validationRepoMock
            .Setup(r =>
                r.GetByRosterIdWithIssuesAsync(_rosterId, _orgId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(validation);

        var query = new GetValidationResultsQuery { RosterId = _rosterId, OrganizationId = _orgId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(404);
        _rosterRepoMock.Verify(
            r =>
                r.GetByIdWithShiftsAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_InProgressValidation_Returns404ForRetry()
    {
        // Arrange
        var validation = new RosterValidation
        {
            Id = _validationId,
            OrganizationId = _orgId,
            RosterId = _rosterId,
            Status = ValidationStatus.InProgress,
        };

        _validationRepoMock
            .Setup(r =>
                r.GetByRosterIdWithIssuesAsync(_rosterId, _orgId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(validation);

        var query = new GetValidationResultsQuery { RosterId = _rosterId, OrganizationId = _orgId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(404);
        _rosterRepoMock.Verify(
            r =>
                r.GetByIdWithShiftsAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }
}
