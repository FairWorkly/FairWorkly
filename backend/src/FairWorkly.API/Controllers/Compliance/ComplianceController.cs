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
}
