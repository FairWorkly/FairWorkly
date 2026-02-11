using FairWorkly.Application.Settings.Features.OrganizationProfile.GetOrganizationProfile;
using FairWorkly.Domain.Common;
using MediatR;

namespace FairWorkly.Application.Settings.Features.OrganizationProfile.UpdateOrganizationProfile;

public record UpdateOrganizationProfileCommand : IRequest<Result<OrganizationProfileDto>>
{
    public string? OrganizationId { get; init; }

    public UpdateOrganizationProfileRequest Request { get; init; } = null!;
}
