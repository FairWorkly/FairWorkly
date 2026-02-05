using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Settings.Features.OrganizationProfile.GetOrganizationProfile;
using FairWorkly.Application.Settings.Features.OrganizationProfile.UpdateOrganizationProfile;
using FairWorkly.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.Controllers.Settings;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizationProfileController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public OrganizationProfileController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet("organization")]
    [Authorize(Policy = "RequireManager")]
    [ProducesResponseType(typeof(OrganizationProfileDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganization(CancellationToken cancellationToken)
    {
        var orgIdString = _currentUserService.OrganizationId;

        if (string.IsNullOrEmpty(orgIdString))
        {
            return Unauthorized(
                new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Missing Organization ID",
                    Detail = "Organization ID not found in authentication token",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Extensions = { ["code"] = "MISSING_ORGANIZATION_ID" },
                }
            );
        }

        if (!Guid.TryParse(orgIdString, out var orgId))
        {
            return BadRequest(
                new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid Organization ID",
                    Detail = "Invalid organization ID format",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Extensions = { ["code"] = "INVALID_ORGANIZATION_ID" },
                }
            );
        }

        var query = new GetOrganizationProfileQuery { OrganizationId = orgId };

        var result = await _mediator.Send(query, cancellationToken);

        if (result.Type == ResultType.NotFound)
        {
            return NotFound(
                new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Organization Not Found",
                    Detail = result.ErrorMessage,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Extensions = { ["code"] = "RESOURCE_NOT_FOUND" },
                }
            );
        }

        return Ok(result.Value);
    }

    [HttpPut("organization")]
    [Authorize(Policy = "RequireAdmin")]
    [ProducesResponseType(typeof(OrganizationProfileDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateOrganization(
        [FromBody] UpdateOrganizationProfileRequest request,
        CancellationToken cancellationToken
    )
    {
        var orgIdString = _currentUserService.OrganizationId;

        if (string.IsNullOrEmpty(orgIdString))
        {
            return Unauthorized(
                new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Missing Organization ID",
                    Detail = "Organization ID not found in authentication token",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Extensions = { ["code"] = "MISSING_ORGANIZATION_ID" },
                }
            );
        }

        if (!Guid.TryParse(orgIdString, out var orgId))
        {
            return BadRequest(
                new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid Organization ID",
                    Detail = "Invalid organization ID format",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Extensions = { ["code"] = "INVALID_ORGANIZATION_ID" },
                }
            );
        }

        var command = new UpdateOrganizationProfileCommand
        {
            OrganizationId = orgId,
            Request = request,
        };

        var result = await _mediator.Send(command, cancellationToken);

        return result.Type switch
        {
            ResultType.Success => Ok(result.Value),

            ResultType.NotFound => NotFound(
                new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Organization Not Found",
                    Detail = result.ErrorMessage,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Extensions = { ["code"] = "RESOURCE_NOT_FOUND" },
                }
            ),

            ResultType.ValidationFailure => BadRequest(
                new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation Failed",
                    Detail = "One or more validation errors occurred",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Extensions =
                    {
                        ["code"] = "VALIDATION_ERROR",
                        ["errors"] = result.ValidationErrors,
                    },
                }
            ),

            _ => Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error",
                detail: "An unexpected error occurred",
                type: "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            ),
        };
    }
}
