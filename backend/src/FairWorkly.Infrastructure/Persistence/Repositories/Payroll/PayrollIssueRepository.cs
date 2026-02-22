using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Domain.Payroll.Entities;

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
}
