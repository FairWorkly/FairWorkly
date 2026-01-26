using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.Controllers.Roster;

[ApiController]
[Route("api/[controller]")]
public class RosterController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
}
