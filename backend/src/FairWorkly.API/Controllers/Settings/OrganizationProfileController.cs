using FairWorkly.API.Controllers;
using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Settings.Features.OrganizationProfile;
using FairWorkly.Application.Settings.Features.OrganizationProfile.GetOrganizationProfile;
using FairWorkly.Application.Settings.Features.OrganizationProfile.UpdateOrganizationProfile;
using FairWorkly.Domain.Common.Result;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.Controllers.Settings;

[Route("api/settings/organization-profile")]
[Authorize(Policy = "RequireAdmin")]
public class OrganizationProfileController(IMediator mediator, ICurrentUserService currentUser)
    : BaseApiController
{
    /// <summary>
    /// Get the current organization's profile.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetOrganization(CancellationToken cancellationToken)
    {
        if (currentUser.OrganizationId is not { } orgId || orgId == Guid.Empty)
        {
            return RespondResult(
                Result<OrganizationProfileDto>.Of401("Organization ID not found in token")
            );
        }

        var query = new GetOrganizationProfileQuery { OrganizationId = orgId };
        var result = await mediator.Send(query, cancellationToken);
        return RespondResult(result);
    }

    /// <summary>
    /// Update the current organization's profile.
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> UpdateOrganization(
        [FromBody] UpdateOrganizationProfileRequest request,
        CancellationToken cancellationToken
    )
    {
        if (currentUser.OrganizationId is not { } orgId || orgId == Guid.Empty)
        {
            return RespondResult(
                Result<OrganizationProfileDto>.Of401("Organization ID not found in token")
            );
        }

        var command = new UpdateOrganizationProfileCommand
        {
            OrganizationId = orgId,
            Request = request,
        };

        var result = await mediator.Send(command, cancellationToken);
        return RespondResult(result);
    }
}
