using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Domain.Payroll.Entities;

namespace FairWorkly.Infrastructure.Persistence.Repositories.Payroll;

public class PayrollValidationRepository : IPayrollValidationRepository
{
    private readonly FairWorklyDbContext _context;

    public PayrollValidationRepository(FairWorklyDbContext context)
    {
        _context = context;
    }

    public void Add(PayrollValidation entity)
    {
        _context.PayrollValidations.Add(entity);
    }
}
