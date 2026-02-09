using FairWorkly.Application.Payroll.Services.Models;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Application.Payroll.Interfaces;

public interface ICsvValidator
{
    Result<List<ValidatedPayrollRow>> Validate(List<string[]> rows, AwardType awardType);
}
