using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Roster.Interfaces;
using FairWorkly.Application.Roster.Services;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Common.Result;
using FairWorkly.Domain.Roster.Entities;
using FairWorkly.Domain.Roster.ValueObjects;
using MediatR;
using RosterEntity = FairWorkly.Domain.Roster.Entities.Roster;

namespace FairWorkly.Application.Roster.Features.ValidateRoster;

/// <summary>
/// Handles roster validation by orchestrating compliance rule execution.
/// Idempotent: returns existing results if validation already completed.
/// </summary>
public class ValidateRosterHandler(
    IRosterRepository rosterRepository,
    IRosterValidationRepository validationRepository,
    IRosterComplianceEngine complianceEngine,
    IUnitOfWork unitOfWork
) : IRequestHandler<ValidateRosterCommand, Result<ValidateRosterResponse>>
{
    public async Task<Result<ValidateRosterResponse>> Handle(
        ValidateRosterCommand request,
        CancellationToken cancellationToken
    )
    {
        static string? ToSafeNotes(string message)
        {
            const int maxLength = 1000;
            if (string.IsNullOrWhiteSpace(message))
                return null;

            var trimmed = message.Trim();
            return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
        }

        // Load roster with shifts and employees
        var roster = await rosterRepository.GetByIdWithShiftsAsync(
            request.RosterId,
            request.OrganizationId,
            cancellationToken
        );

        if (roster == null)
        {
            return Result<ValidateRosterResponse>.Of404("Roster not found");
        }

        // Check for existing completed validation (idempotency)
        var existingValidation = await validationRepository.GetByRosterIdWithIssuesAsync(
            request.RosterId,
            request.OrganizationId,
            cancellationToken
        );

        if (
            existingValidation != null
            && existingValidation.Status is ValidationStatus.Passed or ValidationStatus.Failed
        )
        {
            return Result<ValidateRosterResponse>.Of200(
                "Roster validation completed",
                ValidationResponseBuilder.Build(
                    roster,
                    existingValidation,
                    existingValidation.Issues
                )
            );
        }

        // Reuse stale InProgress record (e.g. previous run crashed) or create new
        RosterValidation validation;
        if (existingValidation != null && existingValidation.Status == ValidationStatus.InProgress)
        {
            validation = existingValidation;
            validation.StartedAt = DateTimeOffset.UtcNow;
            validation.CompletedAt = null;
            validation.Notes = null;
            validation.ExecutedCheckTypes = ExecutedCheckTypeSet.FromCheckTypes(
                complianceEngine.GetExecutedCheckTypes()
            );
            await validationRepository.UpdateAsync(validation, cancellationToken);
        }
        else
        {
            validation = new RosterValidation
            {
                Id = Guid.NewGuid(),
                OrganizationId = request.OrganizationId,
                RosterId = request.RosterId,
                Status = ValidationStatus.InProgress,
                WeekStartDate = roster.WeekStartDate,
                WeekEndDate = roster.WeekEndDate,
                StartedAt = DateTimeOffset.UtcNow,
                ExecutedCheckTypes = ExecutedCheckTypeSet.FromCheckTypes(
                    complianceEngine.GetExecutedCheckTypes()
                ),
            };
            await validationRepository.CreateAsync(validation, cancellationToken);
        }

        // Persist immediately so it's visible to frontend/background queries
        await unitOfWork.SaveChangesAsync(cancellationToken);

        List<RosterIssue> issues;
        try
        {
            // Run compliance checks
            issues = complianceEngine.EvaluateAll(roster.Shifts, validation.Id);

            // Ensure required fields are set on each issue
            foreach (var issue in issues)
            {
                if (issue.Id == Guid.Empty)
                    issue.Id = Guid.NewGuid();

                // Ensure critical foreign keys are always set (don't rely solely on rules)
                issue.OrganizationId = request.OrganizationId;
                issue.RosterValidationId = validation.Id;
            }

            await validationRepository.AddIssuesAsync(issues, cancellationToken);

            // Calculate statistics
            var shiftsWithIssues = issues
                .Where(i => i.ShiftId.HasValue)
                .Select(i => i.ShiftId!.Value)
                .Distinct()
                .Count();

            var employeesWithIssues = issues.Select(i => i.EmployeeId).Distinct().Count();

            // Count issues that cause validation to fail (Error or Critical)
            var failingIssues = issues.Count(i => i.Severity >= IssueSeverity.Error);

            // Update validation record
            validation.TotalShifts = roster.Shifts.Count;
            validation.FailedShifts = shiftsWithIssues;
            validation.PassedShifts = roster.Shifts.Count - shiftsWithIssues;
            validation.TotalIssuesCount = issues.Count;
            validation.CriticalIssuesCount = failingIssues;
            validation.AffectedEmployees = employeesWithIssues;
            validation.Status =
                failingIssues > 0 ? ValidationStatus.Failed : ValidationStatus.Passed;
            validation.CompletedAt = DateTimeOffset.UtcNow;

            await validationRepository.UpdateAsync(validation, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
            when (ex is not OperationCanceledException && !cancellationToken.IsCancellationRequested
            )
        {
            validation.Status = ValidationStatus.Failed;
            validation.CompletedAt = DateTimeOffset.UtcNow;
            validation.Notes = ToSafeNotes($"Validation failed: {ex.Message}");

            await validationRepository.UpdateAsync(validation, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ValidateRosterResponse>.Of422("Roster validation failed.");
        }

        // Build response
        var response = ValidationResponseBuilder.Build(roster, validation, issues);

        return Result<ValidateRosterResponse>.Of200("Roster validation completed", response);
    }
}

/// <summary>
/// Shared helper to build ValidateRosterResponse from roster, validation, and issues.
/// Used by both ValidateRosterHandler and GetValidationResultsHandler.
/// </summary>
public static class ValidationResponseBuilder
{
    public static ValidateRosterResponse Build(
        RosterEntity roster,
        RosterValidation validation,
        IEnumerable<RosterIssue> issues
    )
    {
        var employeeLookup = roster
            .Shifts.Where(s => s.EmployeeId != Guid.Empty && s.Employee != null)
            .Select(s => new
            {
                s.EmployeeId,
                Name = s.Employee!.FullName,
                Number = s.Employee.EmployeeNumber,
            })
            .GroupBy(x => x.EmployeeId)
            .ToDictionary(g => g.Key, g => (Name: g.First().Name, Number: g.First().Number));

        var issueList = issues.ToList();

        return new ValidateRosterResponse
        {
            ValidationId = validation.Id,
            Status = validation.Status,
            TotalShifts = validation.TotalShifts,
            PassedShifts = validation.PassedShifts,
            FailedShifts = validation.FailedShifts,
            TotalIssues = validation.TotalIssuesCount,
            CriticalIssues = validation.CriticalIssuesCount,
            AffectedEmployees = validation.AffectedEmployees,
            WeekStartDate = roster.WeekStartDate,
            WeekEndDate = roster.WeekEndDate,
            TotalEmployees = roster.TotalEmployees,
            ValidatedAt = validation.CompletedAt,
            Issues = issueList
                .Select(i =>
                {
                    employeeLookup.TryGetValue(i.EmployeeId, out var emp);
                    return new RosterIssueSummary
                    {
                        Id = i.Id,
                        ShiftId = i.ShiftId,
                        EmployeeId = i.EmployeeId,
                        EmployeeName = emp.Name,
                        EmployeeNumber = emp.Number,
                        CheckType = i.CheckType.ToString(),
                        Severity = i.Severity,
                        Description = i.Description,
                        ExpectedValue = i.ExpectedValue,
                        ActualValue = i.ActualValue,
                        AffectedDates = i.AffectedDates.ToStorageString(),
                    };
                })
                .ToList(),
        };
    }
}
