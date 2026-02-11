using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Roster.Features.GetRosterDetails;
using FairWorkly.Application.Roster.Features.UploadRoster;
using FairWorkly.Application.Roster.Interfaces;
using FairWorkly.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.Controllers.Roster;

// TODO: [Refactor] Move Result-to-ActionResult mapping out of controllers.
// Create a centralized IActionFilter or MediatR pipeline behavior that maps
// Result<T>.ValidationFailure → ProblemDetails 400, Result<T>.NotFound → 404, etc.
// This eliminates HandleValidationFailureAsync duplication across controllers.
// See GitHub issue for details.
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RosterController(
    IMediator mediator,
    ICurrentUserService currentUser,
    IRosterRepository rosterRepository,
    IFileStorageService fileStorageService
) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly IRosterRepository _rosterRepository = rosterRepository;
    private readonly IFileStorageService _fileStorageService = fileStorageService;

    /// <summary>
    /// Upload roster file for parsing and storage.
    /// </summary>
    /// <param name="file">Roster file (Excel format: .xlsx)</param>
    /// <returns>Roster ID and summary with any warnings</returns>
    [HttpPost("upload")]
    [RequestSizeLimit(52_428_800)] // 50MB
    [ProducesResponseType(typeof(UploadRosterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UploadRosterResponse>> Upload(IFormFile file)
    {
        if (_currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        if (_currentUser.OrganizationId is not { } organizationId || organizationId == Guid.Empty)
        {
            return Unauthorized(new { message = "Organization context not found in token" });
        }

        using var fileStream = file.OpenReadStream();

        var command = new UploadRosterCommand
        {
            FileStream = fileStream,
            FileName = file.FileName,
            FileSize = file.Length,
            ContentType = file.ContentType,
            OrganizationId = organizationId,
            UserId = userId,
        };

        var result = await _mediator.Send(command);

        if (result.Type == ResultType.ValidationFailure)
        {
            return await HandleValidationFailureAsync(result);
        }

        if (result.IsFailure)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get roster details including shift summaries grouped by employee.
    /// Used by the results page after roster upload.
    /// </summary>
    [HttpGet("{rosterId}")]
    [ProducesResponseType(typeof(RosterDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<RosterDetailsResponse>> GetDetails(Guid rosterId)
    {
        if (_currentUser.OrganizationId is not { } organizationId || organizationId == Guid.Empty)
        {
            return Unauthorized(new { message = "Organization context not found in token" });
        }

        var query = new GetRosterDetailsQuery
        {
            RosterId = rosterId,
            OrganizationId = organizationId,
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(new { message = result.ErrorMessage });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Download original roster file from S3.
    /// </summary>
    /// <param name="rosterId">Roster ID</param>
    /// <returns>Original Excel file</returns>
    [HttpGet("{rosterId}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DownloadOriginalFile(Guid rosterId, CancellationToken ct)
    {
        if (_currentUser.OrganizationId is not { } organizationId || organizationId == Guid.Empty)
        {
            return Unauthorized(new { message = "Organization context not found in token" });
        }

        var roster = await _rosterRepository.GetByIdWithShiftsAsync(
            rosterId,
            organizationId,
            ct
        );

        if (roster == null)
        {
            return NotFound(new { message = "Roster not found" });
        }

        if (string.IsNullOrWhiteSpace(roster.OriginalFileS3Key))
        {
            return NotFound(new { message = "Original file not available for this roster" });
        }

        var fileStream = await _fileStorageService.GetFileStreamAsync(
            roster.OriginalFileS3Key,
            ct
        );

        if (fileStream == null)
        {
            return NotFound(new { message = "File not found in storage" });
        }

        var fileName = roster.OriginalFileName ?? "roster.xlsx";
        var contentType = GetContentType(fileName);

        return File(fileStream, contentType, fileName);
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".xls" => "application/vnd.ms-excel",
            ".csv" => "text/csv",
            _ => "application/octet-stream",
        };
    }

    private async Task<ActionResult> HandleValidationFailureAsync<T>(Result<T> result)
    {
        var errors = result.ValidationErrors?
            .GroupBy(e => e.Field)
            .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToArray())
            ?? new Dictionary<string, string[]>();

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Failed",
            Detail = "One or more validation errors occurred.",
            Instance = HttpContext.Request.Path,
        };
        problemDetails.Extensions.Add("errors", errors);

        Response.StatusCode = StatusCodes.Status400BadRequest;
        await Response.WriteAsJsonAsync(problemDetails);
        return new EmptyResult();
    }
}
