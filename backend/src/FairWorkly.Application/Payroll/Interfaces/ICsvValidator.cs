using FairWorkly.Application.Payroll.Services.Models;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Common.Result;

namespace FairWorkly.Application.Payroll.Interfaces;

public interface ICsvValidator
{
    Result<List<ValidatedPayrollRow>> Validate(
        List<string[]> rows,
        AwardType awardType,
        CancellationToken cancellationToken
    );
}
