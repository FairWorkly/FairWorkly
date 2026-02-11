using FairWorkly.Domain.Common;
using MediatR;

namespace FairWorkly.Application.Roster.Features.ValidateRoster;

/// <summary>
/// Command to validate a roster against compliance rules
/// </summary>
public class ValidateRosterCommand : IRequest<Result<ValidateRosterResponse>>
{
    /// <summary>
    /// The roster ID to validate
    /// </summary>
    public Guid RosterId { get; set; }

    /// <summary>
    /// Organization ID for authorization
    /// </summary>
    public Guid OrganizationId { get; set; }
}
