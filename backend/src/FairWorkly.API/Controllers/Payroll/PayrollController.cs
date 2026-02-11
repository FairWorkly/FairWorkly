using FairWorkly.Application.Payroll.Features.ValidatePayroll;
using FairWorkly.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.Controllers.Payroll;

[ApiController]
[Route("api/[controller]")]
public class PayrollController : ControllerBase
{
    private readonly IMediator _mediator;

    public PayrollController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("validation")]
    [Authorize(Policy = "RequireManager")]
    public async Task<IActionResult> Validate(
        IFormFile file,
        [FromForm] string awardType,
        [FromForm] string state,
        [FromForm] bool enableBaseRateCheck = true,
        [FromForm] bool enablePenaltyCheck = true,
        [FromForm] bool enableCasualLoadingCheck = true,
        [FromForm] bool enableSuperCheck = true)
    {
        await using var fileStream = file.OpenReadStream();

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

        if (result.Type == ResultType.ValidationFailure)
            return BadRequest(new { code = 400, msg = "Request validation failed", data = new { errors = result.ValidationErrors } });

        if (result.Type == ResultType.ProcessingFailure)
            return UnprocessableEntity(new { code = 422, msg = result.ErrorMessage, data = new { errors = result.ValidationErrors } });

        if (result.Type == ResultType.Forbidden)
            return StatusCode(403, new { code = 403, msg = result.ErrorMessage });

        if (!result.IsSuccess)
            return StatusCode(500, new { code = 500, msg = "An unexpected error occurred" });

        return Ok(new { code = 200, msg = "Audit completed successfully", data = result.Value });
    }
}
