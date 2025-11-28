using FairWorkly.Application.Compliance.Features.TestArchitecture;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.Controllers.Compliance;

[ApiController]
[Route("api/[controller]")]
public class ComplianceController : ControllerBase
{
    private readonly IMediator _mediator;

    public ComplianceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 架构全链路体检接口
    /// </summary>
    /// <remarks>
    /// 这个接口会同时测试：
    /// 1. 验证管道 (FluentValidation)
    /// 2. 数据库写入 (EF Core + Repository + UnitOfWork)
    /// 3. AI 调用 (Orchestrator + MockClient)
    /// </remarks>
    [HttpPost("test-architecture")]
    [ProducesResponseType(typeof(TestArchitectureResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TestArchitectureResultDto>> TestArchitecture(
        [FromBody] TestArchitectureCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}