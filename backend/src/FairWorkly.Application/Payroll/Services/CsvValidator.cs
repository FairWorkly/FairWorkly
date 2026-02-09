using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Application.Payroll.Services.Models;
using FairWorkly.Domain.Common;

namespace FairWorkly.Application.Payroll.Services;

public class CsvValidator : ICsvValidator
{
    public Result<List<ValidatedPayrollRow>> Validate(List<string[]> rows, string awardType)
    {
        throw new NotImplementedException();
    }
}
