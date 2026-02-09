using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Roster.Entities;
using FairWorkly.Domain.Roster.Enums;

namespace FairWorkly.Domain.Roster.Rules;

/// <summary>
/// Interface for compliance rules that validate roster data against award requirements
/// </summary>
public interface IRosterComplianceRule
{
    /// <summary>
    /// The type of check this rule performs
    /// </summary>
    RosterCheckType CheckType { get; }

    /// <summary>
    /// Evaluates shifts for compliance issues
    /// </summary>
    /// <param name="shifts">The shifts to evaluate</param>
    /// <param name="validationId">The validation batch ID</param>
    /// <returns>List of issues found (empty if compliant)</returns>
    List<RosterIssue> Evaluate(IEnumerable<Shift> shifts, Guid validationId);
}
