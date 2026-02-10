using FairWorkly.Domain.Common;
using MediatR;

namespace FairWorkly.Application.Roster.Features.GetRosterDetails;

/// <summary>
/// Query to retrieve roster details by ID.
/// Returns roster metadata and shift summaries grouped by employee.
/// </summary>
public class GetRosterDetailsQuery : IRequest<Result<RosterDetailsResponse>>
{
    public Guid RosterId { get; set; }
    public Guid OrganizationId { get; set; }
}
