using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Settings.Features.OrganizationProfile.UpdateOrganizationProfile;

public record UpdateOrganizationProfileCommand : IRequest<Result<OrganizationProfileDto>>
{
    public Guid OrganizationId { get; init; }

    public UpdateOrganizationProfileRequest Request { get; init; } = null!;
}
