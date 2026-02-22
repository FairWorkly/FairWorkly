using FairWorkly.Domain.Payroll.Entities;

namespace FairWorkly.Application.Payroll.Interfaces;

public interface IPayslipRepository
{
    void AddRange(IEnumerable<Payslip> payslips);
}
