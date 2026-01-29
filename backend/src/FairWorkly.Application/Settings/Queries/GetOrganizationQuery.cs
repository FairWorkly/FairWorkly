using FairWorkly.Domain.Common;
using MediatR;

namespace FairWorkly.Application.Settings.Queries.GetOrganization;

public record GetOrganizationQuery : IRequest<Result<OrganizationDto>>
{
    public Guid OrganizationId { get; init; }
}
