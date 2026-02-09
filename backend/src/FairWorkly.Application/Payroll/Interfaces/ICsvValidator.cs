using FairWorkly.Application.Payroll.Services.Models;
using FairWorkly.Domain.Common;

namespace FairWorkly.Application.Payroll.Interfaces;

public interface ICsvValidator
{
    Result<List<ValidatedPayrollRow>> Validate(List<string[]> rows, string awardType);
}
