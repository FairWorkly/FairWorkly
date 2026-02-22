using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Domain.Payroll.Entities;

namespace FairWorkly.Infrastructure.Persistence.Repositories.Payroll;

public class PayslipRepository : IPayslipRepository
{
    private readonly FairWorklyDbContext _context;

    public PayslipRepository(FairWorklyDbContext context)
    {
        _context = context;
    }

    public void AddRange(IEnumerable<Payslip> payslips)
    {
        _context.Payslips.AddRange(payslips);
    }
}
