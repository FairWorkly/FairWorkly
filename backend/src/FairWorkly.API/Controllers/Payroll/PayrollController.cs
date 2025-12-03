using MediatR;
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
}
