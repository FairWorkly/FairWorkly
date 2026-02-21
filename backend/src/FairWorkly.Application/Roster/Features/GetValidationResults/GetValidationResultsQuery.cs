using FairWorkly.Application.Roster.Features.ValidateRoster;
using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Roster.Features.GetValidationResults;

/// <summary>
/// Query to retrieve existing validation results for a roster.
/// Returns 404 if no validation has been run yet.
/// </summary>
public class GetValidationResultsQuery : IRequest<Result<ValidateRosterResponse>>
{
    public Guid RosterId { get; set; }
    public Guid OrganizationId { get; set; }
}
