using FairWorkly.Domain.Payroll.Entities;

namespace FairWorkly.Application.Payroll.Interfaces;

public interface IPayrollIssueRepository
{
    void AddRange(IEnumerable<PayrollIssue> issues);

    /// <summary>
    /// Gets a PayrollIssue by ID, scoped to the given organization.
    /// Returns null if not found or belongs to a different organization.
    /// EF change tracking is enabled — caller can directly modify properties and SaveChanges.
    /// </summary>
    Task<PayrollIssue?> GetByIdAsync(
        Guid issueId,
        Guid organizationId,
        CancellationToken cancellationToken = default
    );
}
