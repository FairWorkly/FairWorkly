using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Roster.Interfaces;
using FairWorkly.Application.Roster.Services;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Roster.Entities;
using FairWorkly.Domain.Roster.ValueObjects;
using MediatR;

namespace FairWorkly.Application.Roster.Features.ValidateRoster;

/// <summary>
/// Handles roster validation by orchestrating compliance rule execution
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
            return Result<ValidateRosterResponse>.NotFound("Roster not found");
        }

        var employeeNameById = roster
            .Shifts.Where(s => s.EmployeeId != Guid.Empty && s.Employee != null)
            .Select(s => new { s.EmployeeId, Name = s.Employee!.FullName })
            .GroupBy(x => x.EmployeeId)
            .ToDictionary(g => g.Key, g => g.First().Name);

        string? GetEmployeeName(Guid employeeId)
        {
            return employeeNameById.TryGetValue(employeeId, out var name) ? name : null;
        }

        // Create validation record
        var validation = new RosterValidation
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
        // Persist InProgress record immediately so it's visible to frontend/background queries
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
            validation.Status = failingIssues > 0 ? ValidationStatus.Failed : ValidationStatus.Passed;
            validation.CompletedAt = DateTimeOffset.UtcNow;

            await validationRepository.UpdateAsync(validation, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (
            ex is not OperationCanceledException && !cancellationToken.IsCancellationRequested
        )
        {
            validation.Status = ValidationStatus.Failed;
            validation.CompletedAt = DateTimeOffset.UtcNow;
            validation.Notes = ToSafeNotes($"Validation failed: {ex.Message}");

            await validationRepository.UpdateAsync(validation, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ValidateRosterResponse>.Failure("Roster validation failed.");
        }

        // Build response
        var response = new ValidateRosterResponse
        {
            ValidationId = validation.Id,
            Status = validation.Status,
            TotalShifts = validation.TotalShifts,
            PassedShifts = validation.PassedShifts,
            FailedShifts = validation.FailedShifts,
            TotalIssues = issues.Count,
            CriticalIssues = validation.CriticalIssuesCount,
            Issues = issues.Select(i => new RosterIssueSummary
            {
                Id = i.Id,
                ShiftId = i.ShiftId,
                EmployeeId = i.EmployeeId,
                EmployeeName = GetEmployeeName(i.EmployeeId),
                CheckType = i.CheckType.ToString(),
                Severity = i.Severity,
                Description = i.Description,
                ExpectedValue = i.ExpectedValue,
                ActualValue = i.ActualValue,
                AffectedDates = i.AffectedDates.ToStorageString(),
            }).ToList(),
        };

        return Result<ValidateRosterResponse>.Success(response);
    }
}
