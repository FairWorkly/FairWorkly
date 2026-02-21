using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Roster.Features.ValidateRoster;
using FairWorkly.Application.Roster.Interfaces;
using FairWorkly.Application.Roster.Services;
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

public class ValidateRosterHandlerTests
{
    private readonly Mock<IRosterRepository> _rosterRepoMock = new();
    private readonly Mock<IRosterValidationRepository> _validationRepoMock = new();
    private readonly Mock<IRosterComplianceEngine> _engineMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly Guid _orgId = Guid.NewGuid();
    private readonly Guid _rosterId = Guid.NewGuid();
    private readonly Guid _employeeId = Guid.NewGuid();

    private ValidateRosterHandler CreateHandler()
    {
        return new ValidateRosterHandler(
            _rosterRepoMock.Object,
            _validationRepoMock.Object,
            _engineMock.Object,
            _unitOfWorkMock.Object
        );
    }

    private RosterEntity CreateRoster()
    {
        var weekStart = new DateTime(2026, 2, 2, 0, 0, 0, DateTimeKind.Utc);
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

        var roster = new RosterEntity
        {
            Id = _rosterId,
            OrganizationId = _orgId,
            WeekStartDate = weekStart,
            WeekEndDate = weekStart.AddDays(6),
            WeekNumber = 6,
            Year = 2026,
            TotalEmployees = 1,
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

    [Fact]
    public async Task Handle_ExecutionFailedValidation_RerunsAndSoftDeletesPreviousIssues()
    {
        // Arrange
        var handler = CreateHandler();
        var roster = CreateRoster();
        var existingValidation = new RosterValidation
        {
            Id = Guid.NewGuid(),
            OrganizationId = _orgId,
            RosterId = _rosterId,
            Status = ValidationStatus.Failed,
            Notes = "ExecutionFailure: transient error",
            WeekStartDate = roster.WeekStartDate,
            WeekEndDate = roster.WeekEndDate,
        };
        existingValidation.Issues.Add(
            new RosterIssue
            {
                Id = Guid.NewGuid(),
                OrganizationId = _orgId,
                RosterValidationId = existingValidation.Id,
                EmployeeId = _employeeId,
                CheckType = RosterCheckType.MinimumShiftHours,
                Severity = IssueSeverity.Error,
                Description = "Old issue",
                AffectedDates = AffectedDateSet.Empty,
            }
        );

        _rosterRepoMock
            .Setup(r => r.GetByIdWithShiftsAsync(_rosterId, _orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roster);

        _validationRepoMock
            .Setup(r =>
                r.GetByRosterIdWithIssuesAsync(_rosterId, _orgId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(existingValidation);

        _engineMock
            .Setup(e => e.GetExecutedCheckTypes())
            .Returns(new[] { RosterCheckType.MinimumShiftHours });
        _engineMock
            .Setup(e => e.EvaluateAll(It.IsAny<IEnumerable<Shift>>(), existingValidation.Id))
            .Returns(new List<RosterIssue>());

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new ValidateRosterCommand { RosterId = _rosterId, OrganizationId = _orgId };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Code.Should().Be(200);
        result.Value!.Status.Should().Be(ValidationStatus.Passed);

        _validationRepoMock.Verify(
            r =>
                r.SoftDeleteIssuesAsync(
                    existingValidation.Id,
                    _orgId,
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
        _validationRepoMock.Verify(
            r => r.CreateAsync(It.IsAny<RosterValidation>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _engineMock.Verify(
            e => e.EvaluateAll(It.IsAny<IEnumerable<Shift>>(), existingValidation.Id),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ComplianceFailedValidation_ReturnsCachedResultWithoutRerun()
    {
        // Arrange
        var handler = CreateHandler();
        var roster = CreateRoster();
        var existingValidation = new RosterValidation
        {
            Id = Guid.NewGuid(),
            OrganizationId = _orgId,
            RosterId = _rosterId,
            Status = ValidationStatus.Failed,
            WeekStartDate = roster.WeekStartDate,
            WeekEndDate = roster.WeekEndDate,
            TotalIssuesCount = 1,
            CriticalIssuesCount = 1,
        };
        existingValidation.Issues.Add(
            new RosterIssue
            {
                Id = Guid.NewGuid(),
                OrganizationId = _orgId,
                RosterValidationId = existingValidation.Id,
                EmployeeId = _employeeId,
                CheckType = RosterCheckType.MinimumShiftHours,
                Severity = IssueSeverity.Error,
                Description = "Compliance issue",
                AffectedDates = AffectedDateSet.Empty,
            }
        );

        _rosterRepoMock
            .Setup(r => r.GetByIdWithShiftsAsync(_rosterId, _orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roster);
        _validationRepoMock
            .Setup(r =>
                r.GetByRosterIdWithIssuesAsync(_rosterId, _orgId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(existingValidation);

        var command = new ValidateRosterCommand { RosterId = _rosterId, OrganizationId = _orgId };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Code.Should().Be(200);
        result.Value!.Status.Should().Be(ValidationStatus.Failed);
        result.Value.FailureType.Should().Be(ValidationFailureType.Compliance);
        result.Value.Retriable.Should().BeFalse();

        _engineMock.Verify(
            e => e.EvaluateAll(It.IsAny<IEnumerable<Shift>>(), It.IsAny<Guid>()),
            Times.Never
        );
        _validationRepoMock.Verify(
            r =>
                r.SoftDeleteIssuesAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_EvaluationException_Returns500AndMarksExecutionFailure()
    {
        // Arrange
        var handler = CreateHandler();
        var roster = CreateRoster();

        _rosterRepoMock
            .Setup(r => r.GetByIdWithShiftsAsync(_rosterId, _orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roster);
        _validationRepoMock
            .Setup(r =>
                r.GetByRosterIdWithIssuesAsync(_rosterId, _orgId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((RosterValidation?)null);
        _validationRepoMock
            .Setup(r => r.CreateAsync(It.IsAny<RosterValidation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RosterValidation v, CancellationToken _) => v);

        RosterValidation? updatedValidation = null;
        _validationRepoMock
            .Setup(r => r.UpdateAsync(It.IsAny<RosterValidation>(), It.IsAny<CancellationToken>()))
            .Callback<RosterValidation, CancellationToken>((v, _) => updatedValidation = v)
            .Returns(Task.CompletedTask);

        _engineMock
            .Setup(e => e.GetExecutedCheckTypes())
            .Returns(new[] { RosterCheckType.MinimumShiftHours });
        _engineMock
            .Setup(e => e.EvaluateAll(It.IsAny<IEnumerable<Shift>>(), It.IsAny<Guid>()))
            .Throws(new InvalidOperationException("Engine crashed"));

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new ValidateRosterCommand { RosterId = _rosterId, OrganizationId = _orgId };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(500);
        updatedValidation.Should().NotBeNull();
        updatedValidation!.Status.Should().Be(ValidationStatus.Failed);
        updatedValidation.Notes.Should().StartWith("ExecutionFailure:");
    }
}
