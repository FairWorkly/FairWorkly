using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Payroll.Features.ValidatePayroll;

public class ValidatePayrollCommand : IRequest<Result<ValidatePayrollDto>>
{
    public Stream FileStream { get; set; } = null!;
    public string FileName { get; set; } = "";
    public long FileSize { get; set; }
    public string AwardType { get; set; } = "";
    public string State { get; set; } = "VIC";
    public bool EnableBaseRateCheck { get; set; } = true;
    public bool EnablePenaltyCheck { get; set; } = true;
    public bool EnableCasualLoadingCheck { get; set; } = true;
    public bool EnableSuperCheck { get; set; } = true;
}
