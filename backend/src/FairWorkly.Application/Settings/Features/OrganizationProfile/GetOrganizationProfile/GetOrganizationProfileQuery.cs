using FairWorkly.Domain.Common;
using MediatR;

namespace FairWorkly.Application.Settings.Features.OrganizationProfile.GetOrganizationProfile;

public record GetOrganizationProfileQuery : IRequest<Result<OrganizationProfileDto>>
{
    public Guid OrganizationId { get; init; }
}
