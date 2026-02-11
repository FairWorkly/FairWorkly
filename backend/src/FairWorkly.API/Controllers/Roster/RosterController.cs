using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Roster.Features.GetRosterDetails;
using FairWorkly.Application.Roster.Features.UploadRoster;
using FairWorkly.Application.Roster.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.Controllers.Roster;

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
    public async Task<ActionResult<UploadRosterResponse>> Upload([FromForm] IFormFile file)
    {
        if (!Guid.TryParse(_currentUser.UserId, out var userId) || userId == Guid.Empty)
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        if (!Guid.TryParse(_currentUser.OrganizationId, out var organizationId) || organizationId == Guid.Empty)
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

        return new ObjectResult(result);
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
        if (!Guid.TryParse(_currentUser.OrganizationId, out var organizationId) || organizationId == Guid.Empty)
        {
            return Unauthorized(new { message = "Organization context not found in token" });
        }

        var query = new GetRosterDetailsQuery
        {
            RosterId = rosterId,
            OrganizationId = organizationId,
        };

        var result = await _mediator.Send(query);

        return new ObjectResult(result);
    }

    /// <summary>
    /// Download original roster file from S3.
    /// </summary>
    /// <param name="rosterId">Roster ID</param>
    /// <returns>Original Excel file</returns>
    [HttpGet("{rosterId}/download")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DownloadOriginalFile(Guid rosterId, CancellationToken ct)
    {
        if (!Guid.TryParse(_currentUser.OrganizationId, out var organizationId) || organizationId == Guid.Empty)
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

}
