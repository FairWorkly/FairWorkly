using FairWorkly.Application.Settings.Features.AcceptInvitation;
using FairWorkly.Application.Settings.Features.ValidateInvitationToken;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.Controllers.Auth;

[Route("api/invite")]
public class InvitationController(IMediator mediator) : BaseApiController
{
    /// <summary>
    /// Validate an invitation token without consuming it. Public endpoint.
    /// Returns invitee info if valid, or an error if expired/invalid/already accepted.
    /// </summary>
    [HttpGet("validate")]
    [AllowAnonymous]
    public async Task<IActionResult> ValidateToken(
        [FromQuery] string token,
        CancellationToken cancellationToken
    )
    {
        var query = new ValidateInvitationTokenQuery { Token = token };
        var result = await mediator.Send(query, cancellationToken);
        return RespondResult(result);
    }

    /// <summary>
    /// Accept an invitation and set password. Public endpoint (no auth required).
    /// </summary>
    [HttpPost("accept")]
    [AllowAnonymous]
    public async Task<IActionResult> AcceptInvitation(
        [FromBody] AcceptInvitationRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new AcceptInvitationCommand
        {
            Token = request.Token,
            Password = request.Password,
        };

        var result = await mediator.Send(command, cancellationToken);
        return RespondResult(result);
    }
}
