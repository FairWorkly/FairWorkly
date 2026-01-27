// FairWorkly.Application/Settings/Queries/GetOrganization/GetOrganizationQueryHandler.cs

using MediatR;
using FairWorkly.Domain.Auth.Interfaces;

namespace FairWorkly.Application.Settings.Queries.GetOrganization;

/// <summary>
/// Handler for GetOrganizationQuery
/// Retrieves organization profile from database and maps to DTO
/// </summary>
public class GetOrganizationQueryHandler 
    : IRequestHandler<GetOrganizationQuery, OrganizationDto>
{
    private readonly IOrganizationRepository _organizationRepository;

    public GetOrganizationQueryHandler(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<OrganizationDto> Handle(
        GetOrganizationQuery request, 
        CancellationToken cancellationToken)
    {
        // Step 1: Fetch organization from repository
        var organization = await _organizationRepository.GetByIdAsync(
            request.OrganizationId, 
            cancellationToken);

        // Step 2: Validate organization exists
        if (organization == null)
        {
            throw new NotFoundException(
                $"Organization with ID {request.OrganizationId} not found");
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

        return dto;
    }
}