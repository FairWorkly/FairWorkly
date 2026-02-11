using System.Security.Claims;
using FairWorkly.Application.Awards.Features.GetOrganizationAwards;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.Controllers.Awards;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AwardsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get awards configured for the current user's organization.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(GetOrganizationAwardsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetOrganizationAwardsResponse>> GetOrganizationAwards()
    {
        var orgIdClaim = User.FindFirstValue("orgId");
        if (!Guid.TryParse(orgIdClaim, out var organizationId) || organizationId == Guid.Empty)
        {
            return Unauthorized(new { message = "Organization context not found in token" });
        }

        var query = new GetOrganizationAwardsQuery { OrganizationId = organizationId };
        var result = await mediator.Send(query);

        return new ObjectResult(result);
    }
}
