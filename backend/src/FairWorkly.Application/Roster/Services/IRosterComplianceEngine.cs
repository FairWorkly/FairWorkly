using FairWorkly.Domain.Roster.Entities;
using FairWorkly.Domain.Roster.Enums;

namespace FairWorkly.Application.Roster.Services;

/// <summary>
/// Service interface for running roster compliance checks
/// </summary>
public interface IRosterComplianceEngine
{
    /// <summary>
    /// The set of check types this engine will run (based on registered rules).
    /// </summary>
    IReadOnlyList<RosterCheckType> GetExecutedCheckTypes();

    /// <summary>
    /// Runs all compliance rules against the provided shifts
    /// </summary>
    /// <param name="shifts">The shifts to validate</param>
    /// <param name="validationId">The validation batch ID</param>
    /// <returns>List of all issues found across all rules</returns>
    List<RosterIssue> EvaluateAll(IEnumerable<Shift> shifts, Guid validationId);
}
