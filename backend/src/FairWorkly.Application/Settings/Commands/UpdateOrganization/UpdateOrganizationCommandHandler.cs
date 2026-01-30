using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using MediatR;

namespace FairWorkly.Application.Settings.Commands.UpdateOrganization;

public class UpdateOrganizationCommandHandler
    : IRequestHandler<UpdateOrganizationCommand, Result<OrganizationDto>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrganizationCommandHandler(
        IOrganizationRepository organizationRepository,
        IUnitOfWork unitOfWork
    )
    {
        _organizationRepository = organizationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrganizationDto>> Handle(
        UpdateOrganizationCommand command,
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
            return Result<OrganizationDto>.NotFound(
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
        if (!Enum.TryParse<AustralianState>(request.State, ignoreCase: true, out var stateEnum))
        {
            return Result<OrganizationDto>.ValidationFailure(
                "Invalid state code",
                new List<ValidationError>
                {
                    new() { Field = "State", Message = "Invalid Australian state code" },
                }
            );
        }
        organization.State = stateEnum;

        organization.Postcode = request.Postcode;
        organization.LogoUrl = request.LogoUrl;

        // Step 4: Persist changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Step 5: Map updated entity to DTO and return
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
            State = organization.State.ToString(),
            Postcode = organization.Postcode,
            LogoUrl = organization.LogoUrl,
        };

        return Result<OrganizationDto>.Success(dto);
    }
}
