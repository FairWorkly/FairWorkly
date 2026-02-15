using FairWorkly.Domain.Payroll.Entities;

namespace FairWorkly.Application.Payroll.Interfaces;

public interface IPayrollIssueRepository
{
    void AddRange(IEnumerable<PayrollIssue> issues);
}
