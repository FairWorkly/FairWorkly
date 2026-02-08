using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common;
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
        var orgId = Guid.Parse(request.OrganizationId!);
        // Step 1: Fetch organization from repository
        var organization = await _organizationRepository.GetByIdAsync(orgId, cancellationToken);

        // Step 2: Validate organization exists
        if (organization == null)
        {
            return Result<OrganizationProfileDto>.NotFound($"Organization {orgId} not found");
        }

        // Step 3: Map entity to DTO
        var dto = new OrganizationProfileDto
        {
            CompanyName = organization.CompanyName,
            ABN = organization.ABN,
            IndustryType = organization.IndustryType,
            ContactEmail = organization.ContactEmail,
            PhoneNumber = organization.PhoneNumber,
            AddressLine1 = organization.AddressLine1,
            AddressLine2 = organization.AddressLine2,
            Suburb = organization.Suburb,
            State = organization.State.ToString(),
            Postcode = organization.Postcode,
            LogoUrl = organization.LogoUrl,
        };

        return Result<OrganizationProfileDto>.Success(dto);
    }
}
