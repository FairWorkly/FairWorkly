using FairWorkly.Domain.Payroll.Entities;

namespace FairWorkly.Application.Payroll.Interfaces;

public interface IPayrollValidationRepository
{
    void Add(PayrollValidation entity);
}
