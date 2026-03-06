using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Settings.Features.OrganizationProfile.GetOrganizationProfile;

public class GetOrganizationProfileHandler
    : IRequestHandler<GetOrganizationProfileQuery, Result<OrganizationProfileDto>>
{
    private readonly IOrganizationRepository _organizationRepository;

    public GetOrganizationProfileHandler(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<Result<OrganizationProfileDto>> Handle(
        GetOrganizationProfileQuery request,
        CancellationToken cancellationToken
    )
    {
        var organization = await _organizationRepository.GetByIdWithAwardsAsync(
            request.OrganizationId,
            cancellationToken
        );

        if (organization == null)
        {
            return Result<OrganizationProfileDto>.Of404(
                $"Organization {request.OrganizationId} not found"
            );
        }

        return Result<OrganizationProfileDto>.Of200(
            "Organization profile retrieved",
            OrganizationProfileDto.FromEntity(organization)
        );
    }
}
