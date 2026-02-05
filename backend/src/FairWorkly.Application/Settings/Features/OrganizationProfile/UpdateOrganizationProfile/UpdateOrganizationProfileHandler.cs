using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Settings.Features.OrganizationProfile.GetOrganizationProfile;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using MediatR;

namespace FairWorkly.Application.Settings.Features.OrganizationProfile.UpdateOrganizationProfile;

public class UpdateOrganizationProfileHandler
    : IRequestHandler<UpdateOrganizationProfileCommand, Result<OrganizationProfileDto>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrganizationProfileHandler(
        IOrganizationRepository organizationRepository,
        IUnitOfWork unitOfWork
    )
    {
        _organizationRepository = organizationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrganizationProfileDto>> Handle(
        UpdateOrganizationProfileCommand command,
        CancellationToken cancellationToken
    )
    {
        var request = command.Request;

        // Step 1: Fetch organization from repository
        var organization = await _organizationRepository.GetByIdAsync(
            command.OrganizationId,
            cancellationToken
        );

        // Step 2: Validate organization exists
        if (organization == null)
        {
            return Result<OrganizationProfileDto>.NotFound(
                $"Organization {command.OrganizationId} not found"
            );
        }

        // Step 3: Update organization fields
        organization.CompanyName = request.CompanyName;
        organization.ABN = request.ABN;
        organization.IndustryType = request.IndustryType;
        organization.ContactEmail = request.ContactEmail;
        organization.PhoneNumber = request.PhoneNumber;
        organization.AddressLine1 = request.AddressLine1;
        organization.AddressLine2 = request.AddressLine2;
        organization.Suburb = request.Suburb;

        // Convert State string to enum
        var stateEnum = Enum.Parse<AustralianState>(request.State, ignoreCase: true);
        organization.State = stateEnum;

        organization.Postcode = request.Postcode;
        organization.LogoUrl = request.LogoUrl;

        // Step 4: Persist changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Step 5: Map updated entity to DTO and return
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
