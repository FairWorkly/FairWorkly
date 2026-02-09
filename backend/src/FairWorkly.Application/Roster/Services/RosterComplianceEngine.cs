using FairWorkly.Domain.Roster.Entities;
using FairWorkly.Domain.Roster.Enums;
using FairWorkly.Domain.Roster.Rules;

namespace FairWorkly.Application.Roster.Services;

/// <summary>
/// Orchestrates roster compliance validation by running all registered rules
/// </summary>
public class RosterComplianceEngine : IRosterComplianceEngine
{
    private readonly IEnumerable<IRosterComplianceRule> _rules;

    public RosterComplianceEngine(IEnumerable<IRosterComplianceRule> rules)
    {
        _rules = rules;
    }

    public IReadOnlyList<RosterCheckType> GetExecutedCheckTypes()
    {
        return _rules.Select(r => r.CheckType).Distinct().OrderBy(c => (int)c).ToArray();
    }

    /// <summary>
    /// Runs all compliance rules against the provided shifts
    /// </summary>
    public List<RosterIssue> EvaluateAll(IEnumerable<Shift> shifts, Guid validationId)
    {
        var allIssues = new List<RosterIssue>();
        var shiftsList = shifts.ToList();

        foreach (var rule in _rules)
        {
            var issues = rule.Evaluate(shiftsList, validationId);
            allIssues.AddRange(issues);
        }

        return allIssues;
    }
}
