using FairWorkly.Application.Payroll.Features.ValidatePayroll;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.Controllers.Payroll;

[Route("api/[controller]")]
public class PayrollController : BaseApiController
{
    private readonly IMediator _mediator;

    public PayrollController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("validation")]
    [Authorize(Policy = "RequireManager")]
    [RequestSizeLimit(2_097_152)] // 2MB
    public async Task<IActionResult> Validate(
        IFormFile file,
        [FromForm] string awardType,
        [FromForm] string state,
        [FromForm] bool enableBaseRateCheck = true,
        [FromForm] bool enablePenaltyCheck = true,
        [FromForm] bool enableCasualLoadingCheck = true,
        [FromForm] bool enableSuperCheck = true)
    {
        var fileStream = file.OpenReadStream();

        var command = new ValidatePayrollCommand
        {
            FileStream = fileStream,
            FileName = file.FileName,
            FileSize = file.Length,
            AwardType = awardType,
            State = state,
            EnableBaseRateCheck = enableBaseRateCheck,
            EnablePenaltyCheck = enablePenaltyCheck,
            EnableCasualLoadingCheck = enableCasualLoadingCheck,
            EnableSuperCheck = enableSuperCheck,
        };

        var result = await _mediator.Send(command);

        return RespondResult(result);
    }
}
