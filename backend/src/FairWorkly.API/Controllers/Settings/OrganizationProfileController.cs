using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Settings.Features.OrganizationProfile.GetOrganizationProfile;
using FairWorkly.Application.Settings.Features.OrganizationProfile.UpdateOrganizationProfile;
using FairWorkly.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.Controllers.Settings;

[ApiController]
[Route("api/settings")]
[Authorize(Policy = "RequireAdmin")]
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
    [ProducesResponseType(typeof(OrganizationProfileDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganization(CancellationToken cancellationToken)
    {
        var query = new GetOrganizationProfileQuery
        {
            OrganizationId = _currentUserService.OrganizationId,
        };

        var result = await _mediator.Send(query, cancellationToken);

        return MapResultToResponse(result);
    }

    [HttpPut("organization")]
    [ProducesResponseType(typeof(OrganizationProfileDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateOrganization(
        [FromBody] UpdateOrganizationProfileRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new UpdateOrganizationProfileCommand
        {
            OrganizationId = _currentUserService.OrganizationId,
            Request = request,
        };

        var result = await _mediator.Send(command, cancellationToken);

        return MapResultToResponse(result);
    }

    private IActionResult MapResultToResponse(Result<OrganizationProfileDto> result)
    {
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
                    Detail = result.ErrorMessage,
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
