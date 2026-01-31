using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Settings.Commands.UpdateOrganization;
using FairWorkly.Application.Settings.Queries.GetOrganization;
using FairWorkly.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public SettingsController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet("organization")]
    [ProducesResponseType(typeof(OrganizationDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganization()
    {
        var orgIdString = _currentUserService.OrganizationId;

        if (string.IsNullOrEmpty(orgIdString))
        {
            return Unauthorized(
                new
                {
                    code = "MISSING_ORGANIZATION_ID",
                    message = "Organization ID not found in authentication token",
                }
            );
        }

        if (!Guid.TryParse(orgIdString, out var orgId))
        {
            return BadRequest(
                new { code = "INVALID_ORGANIZATION_ID", message = "Invalid organization ID format" }
            );
        }

        var query = new GetOrganizationQuery { OrganizationId = orgId };

        var result = await _mediator.Send(query);

        if (result.Type == ResultType.NotFound)
        {
            return NotFound(new { code = "RESOURCE_NOT_FOUND", message = result.ErrorMessage });
        }

        return Ok(result.Value);
    }

    [HttpPut("organization")]
    [ProducesResponseType(typeof(OrganizationDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateOrganization(
        [FromBody] UpdateOrganizationRequest request
    )
    {
        var orgIdString = _currentUserService.OrganizationId;

        if (string.IsNullOrEmpty(orgIdString))
        {
            return Unauthorized(
                new
                {
                    code = "MISSING_ORGANIZATION_ID",
                    message = "Organization ID not found in authentication token",
                }
            );
        }

        if (!Guid.TryParse(orgIdString, out var orgId))
        {
            return BadRequest(
                new { code = "INVALID_ORGANIZATION_ID", message = "Invalid organization ID format" }
            );
        }

        var command = new UpdateOrganizationCommand { OrganizationId = orgId, Request = request };

        var result = await _mediator.Send(command);

        return result.Type switch
        {
            ResultType.Success => Ok(result.Value),

            ResultType.NotFound => NotFound(
                new { code = "RESOURCE_NOT_FOUND", message = result.ErrorMessage }
            ),

            ResultType.ValidationFailure => BadRequest(
                new
                {
                    code = "VALIDATION_ERROR",
                    message = "Validation failed",
                    errors = result.ValidationErrors,
                }
            ),

            _ => StatusCode(
                StatusCodes.Status500InternalServerError,
                new { code = "INTERNAL_ERROR", message = "An unexpected error occurred" }
            ),
        };
    }
}
