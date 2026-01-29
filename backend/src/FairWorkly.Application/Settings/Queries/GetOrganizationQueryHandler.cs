using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common;
using MediatR;

namespace FairWorkly.Application.Settings.Queries.GetOrganization;

public class GetOrganizationQueryHandler
    : IRequestHandler<GetOrganizationQuery, Result<OrganizationDto>>
{
    private readonly IOrganizationRepository _organizationRepository;

    public GetOrganizationQueryHandler(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<Result<OrganizationDto>> Handle(
        GetOrganizationQuery request,
        CancellationToken cancellationToken
    )
    {
        // Step 1: Fetch organization from repository
        var organization = await _organizationRepository.GetByIdAsync(
            request.OrganizationId,
            cancellationToken
        );

        // Step 2: Validate organization exists
        if (organization == null)
        {
            return Result<OrganizationDto>.NotFound(
                $"Organization {request.OrganizationId} not found"
            );
        }

        // Step 3: Map entity to DTO
        var dto = new OrganizationDto
        {
            CompanyName = organization.CompanyName,
            ABN = organization.ABN,
            IndustryType = organization.IndustryType,
            ContactEmail = organization.ContactEmail,
            PhoneNumber = organization.PhoneNumber,
            AddressLine1 = organization.AddressLine1,
            AddressLine2 = organization.AddressLine2,
            Suburb = organization.Suburb,
            State = organization.State.ToString(), // Enum → String
            Postcode = organization.Postcode,
            LogoUrl = organization.LogoUrl,
        };

        return Result<OrganizationDto>.Success(dto);
    }
}
