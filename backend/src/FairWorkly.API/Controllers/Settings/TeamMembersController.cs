using FairWorkly.API.Controllers;
using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Settings.Features.CancelInvitation;
using FairWorkly.Application.Settings.Features.GetTeamMembers;
using FairWorkly.Application.Settings.Features.InviteTeamMember;
using FairWorkly.Application.Settings.Features.ResendInvitation;
using FairWorkly.Application.Settings.Features.UpdateTeamMember;
using FairWorkly.Domain.Common.Result;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.Controllers.Settings;

[Route("api/settings/team")]
[Authorize(Policy = "RequireAdmin")]
public class TeamMembersController(IMediator mediator, ICurrentUserService currentUser)
    : BaseApiController
{
    /// <summary>
    /// Get all team members in the current organization.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetTeamMembers(CancellationToken cancellationToken)
    {
        if (currentUser.OrganizationId is not { } orgId || orgId == Guid.Empty)
            return RespondResult(
                Result<List<TeamMemberDto>>.Of401("Organization ID not found in token")
            );

        var query = new GetTeamMembersQuery { OrganizationId = orgId };
        var result = await mediator.Send(query, cancellationToken);
        return RespondResult(result);
    }

    /// <summary>
    /// Update a team member's role or status.
    /// </summary>
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateTeamMember(
        Guid id,
        [FromBody] UpdateTeamMemberRequest request,
        CancellationToken cancellationToken
    )
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
            return RespondResult(Result<TeamMemberUpdatedDto>.Of401("User ID not found in token"));

        if (currentUser.OrganizationId is not { } orgId || orgId == Guid.Empty)
            return RespondResult(
                Result<TeamMemberUpdatedDto>.Of401("Organization ID not found in token")
            );

        var command = new UpdateTeamMemberCommand
        {
            CurrentUserId = userId,
            OrganizationId = orgId,
            TargetUserId = id,
            Role = request.Role,
            IsActive = request.IsActive,
        };

        var result = await mediator.Send(command, cancellationToken);
        return RespondResult(result);
    }

    /// <summary>
    /// Invite a new team member to the organization.
    /// </summary>
    [HttpPost("invite")]
    public async Task<IActionResult> InviteTeamMember(
        [FromBody] InviteTeamMemberRequest request,
        CancellationToken cancellationToken
    )
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
            return RespondResult(
                Result<InviteTeamMemberResponse>.Of401("User ID not found in token")
            );

        if (currentUser.OrganizationId is not { } orgId || orgId == Guid.Empty)
            return RespondResult(
                Result<InviteTeamMemberResponse>.Of401("Organization ID not found in token")
            );

        var command = new InviteTeamMemberCommand
        {
            OrganizationId = orgId,
            InvitedByUserId = userId,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role,
        };

        var result = await mediator.Send(command, cancellationToken);
        return RespondResult(result);
    }

    /// <summary>
    /// Resend invitation to a pending team member.
    /// </summary>
    [HttpPost("{id:guid}/resend-invite")]
    public async Task<IActionResult> ResendInvitation(Guid id, CancellationToken cancellationToken)
    {
        if (currentUser.OrganizationId is not { } orgId || orgId == Guid.Empty)
            return RespondResult(
                Result<ResendInvitationResponse>.Of401("Organization ID not found in token")
            );

        var command = new ResendInvitationCommand { OrganizationId = orgId, TargetUserId = id };

        var result = await mediator.Send(command, cancellationToken);
        return RespondResult(result);
    }

    /// <summary>
    /// Cancel a pending invitation and remove the user.
    /// </summary>
    [HttpDelete("{id:guid}/invite")]
    public async Task<IActionResult> CancelInvitation(Guid id, CancellationToken cancellationToken)
    {
        if (currentUser.OrganizationId is not { } orgId || orgId == Guid.Empty)
            return RespondResult(
                Result<CancelInvitationResponse>.Of401("Organization ID not found in token")
            );

        var command = new CancelInvitationCommand { OrganizationId = orgId, TargetUserId = id };

        var result = await mediator.Send(command, cancellationToken);
        return RespondResult(result);
    }
}
