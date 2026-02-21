using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Roster.Features.UploadRoster;

/// <summary>
/// Command to upload and parse a roster Excel file.
/// Triggers the complete roster upload flow:
/// 1. Upload file to S3
/// 2. Parse file via Agent Service
/// 3. Match employees by number/email
/// 4. Create Roster and Shift entities
/// 5. Save to database
/// 6. Return response with non-blocking warnings (reserved; usually empty under current policy)
/// </summary>
public class UploadRosterCommand : IRequest<Result<UploadRosterResponse>>
{
    /// <summary>
    /// Stream of the uploaded roster file (Excel format: .xlsx or .xls).
    /// Caller is responsible for disposing the stream.
    /// </summary>
    public Stream FileStream { get; set; } = null!;

    /// <summary>
    /// Original filename of the uploaded file.
    /// </summary>
    public string FileName { get; set; } = null!;

    /// <summary>
    /// Size of the file in bytes.
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Content type of the file (e.g., "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet").
    /// </summary>
    public string ContentType { get; set; } = null!;

    /// <summary>
    /// Organization ID from authenticated user's JWT claims.
    /// Used to scope roster and match employees.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// User ID who uploaded the roster (from JWT claims).
    /// Used for audit trail (CreatedBy field).
    /// </summary>
    public Guid UserId { get; set; }
}
