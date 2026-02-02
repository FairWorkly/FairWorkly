using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FairWorkly.Domain.Common;
using FairWorkly.Application.Settings.Features.GetTeamMembers;
using FairWorkly.Application.Settings.Features.UpdateTeamMember;

namespace FairWorkly.API.Controllers.Settings;

[ApiController]
[Route("api/settings")]
[Authorize]
public class SettingsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get all team members in the current organization
    /// </summary>
    [HttpGet("team")]
    public async Task<ActionResult<List<TeamMemberDto>>> GetTeamMembers()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var query = new GetTeamMembersQuery { CurrentUserId = userId.Value };
        var result = await mediator.Send(query);

        if (result.IsFailure)
            return Unauthorized(new { message = result.ErrorMessage });

        return Ok(result.Value);
    }

    /// <summary>
    /// Update a team member's role or status (Admin only)
    /// </summary>
    [HttpPatch("team/{id:guid}")]
    [Authorize(Policy = "RequireAdmin")]
    public async Task<ActionResult<TeamMemberUpdatedDto>> UpdateTeamMember(
        Guid id,
        [FromBody] UpdateTeamMemberRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var command = new UpdateTeamMemberCommand
        {
            CurrentUserId = userId.Value,
            TargetUserId = id,
            Role = request.Role,
            IsActive = request.IsActive
        };

        var result = await mediator.Send(command);

        if (result.Type == ResultType.ValidationFailure)
            return BadRequest(new { code = "VALIDATION_ERROR", errors = result.ValidationErrors });

        if (result.Type == ResultType.NotFound)
            return NotFound(new { message = result.ErrorMessage });

        if (result.Type == ResultType.Forbidden)
            return StatusCode(403, new { message = result.ErrorMessage });

        if (result.IsFailure)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Value);
    }

    private Guid? GetCurrentUserId()
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (Guid.TryParse(sub, out var userId) && userId != Guid.Empty)
            return userId;

        return null;
    }
}

/// <summary>
/// Request body for PATCH /settings/team/{id}
/// </summary>
public class UpdateTeamMemberRequest
{
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
}
