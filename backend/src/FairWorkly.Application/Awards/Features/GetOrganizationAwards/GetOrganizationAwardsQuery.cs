using FairWorkly.Domain.Common;
using MediatR;

namespace FairWorkly.Application.Awards.Features.GetOrganizationAwards;

public class GetOrganizationAwardsQuery : IRequest<Result<GetOrganizationAwardsResponse>>
{
    public Guid OrganizationId { get; set; }
}
