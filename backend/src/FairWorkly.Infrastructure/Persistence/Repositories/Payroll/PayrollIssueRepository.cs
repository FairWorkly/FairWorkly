using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Domain.Payroll.Entities;
using Microsoft.EntityFrameworkCore;

namespace FairWorkly.Infrastructure.Persistence.Repositories.Payroll;

public class PayrollIssueRepository : IPayrollIssueRepository
{
    private readonly FairWorklyDbContext _context;

    public PayrollIssueRepository(FairWorklyDbContext context)
    {
        _context = context;
    }

    public void AddRange(IEnumerable<PayrollIssue> issues)
    {
        _context.PayrollIssues.AddRange(issues);
    }

    public async Task<PayrollIssue?> GetByIdAsync(
        Guid issueId,
        Guid organizationId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context.PayrollIssues.FirstOrDefaultAsync(
            i => i.Id == issueId && i.OrganizationId == organizationId,
            cancellationToken
        );
    }
}
