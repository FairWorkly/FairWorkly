using System.Security.Claims;
using FairWorkly.Application.Settings.Features.GetTeamMembers;
using FairWorkly.Application.Settings.Features.UpdateTeamMember;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.Controllers.Settings;

[ApiController]
[Route("api/[controller]")]
[Authorize] // All endpoints require authentication
public class SettingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SettingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all team members for the current user's organization.
    /// </summary>
    /// <returns>List of team members</returns>
    [HttpGet("team")]
    [ProducesResponseType(typeof(List<TeamMemberDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TeamMemberDto>>> GetTeamMembers()
    {
        // Extract organization ID from JWT claims
        var organizationId = GetOrganizationIdFromClaims();

        var query = new GetTeamMembersQuery { OrganizationId = organizationId };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Update a team member's role or status.
    /// Only Admin users can perform this action.
    /// </summary>
    /// <param name="id">User ID to update</param>
    /// <param name="request">Update data</param>
    /// <returns>Updated team member data</returns>
    [HttpPatch("team/{id:guid}")]
    [Authorize(Policy = "RequireAdmin")] // Only admins can update
    [ProducesResponseType(typeof(UpdateTeamMemberResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UpdateTeamMemberResult>> UpdateTeamMember(
        Guid id,
        [FromBody] UpdateTeamMemberRequest request
    )
    {
        var organizationId = GetOrganizationIdFromClaims();

        var command = new UpdateTeamMemberCommand
        {
            UserId = id,
            Role = request.Role,
            IsActive = request.IsActive,
            RequestingUserOrganizationId = organizationId,
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Helper to extract OrganizationId from JWT claims.
    /// </summary>
    private Guid GetOrganizationIdFromClaims()
    {
        var claim = User.FindFirst("OrganizationId") ?? User.FindFirst("organization_id");

        if (claim == null || !Guid.TryParse(claim.Value, out var orgId))
        {
            throw new UnauthorizedAccessException("Organization context not found in token");
        }

        return orgId;
    }
}

/// <summary>
/// Request body for PATCH /api/settings/team/{id}
/// </summary>
public class UpdateTeamMemberRequest
{
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
}
