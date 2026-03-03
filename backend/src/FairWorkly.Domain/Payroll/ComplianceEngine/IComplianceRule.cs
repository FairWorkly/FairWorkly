using FairWorkly.Domain.Payroll.Entities;
using FairWorkly.Domain.Payroll.Enums;

namespace FairWorkly.Domain.Payroll.ComplianceEngine;

/// <summary>
/// Interface for compliance rules that validate payslip data against award requirements
/// </summary>
public interface IComplianceRule
{
    /// <summary>
    /// Name of this rule for identification and logging
    /// </summary>
    string RuleName { get; }

    /// <summary>
    /// Category of this rule, used for enable/disable filtering by Handler
    /// </summary>
    IssueCategory Category { get; }

    /// <summary>
    /// Evaluates a payslip for compliance issues
    /// </summary>
    /// <param name="payslip">The payslip to evaluate</param>
    /// <param name="validationId">The validation batch ID</param>
    /// <returns>List of issues found (empty if compliant)</returns>
    List<PayrollIssue> Evaluate(Payslip payslip, Guid validationId);
}
