using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Common.Result;
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

        var organization = await _organizationRepository.GetByIdWithAwardsAsync(
            command.OrganizationId,
            cancellationToken
        );

        if (organization == null)
        {
            return Result<OrganizationProfileDto>.Of404(
                $"Organization {command.OrganizationId} not found"
            );
        }

        if (!Enum.TryParse<AustralianState>(request.State, ignoreCase: true, out var stateEnum))
        {
            return Result<OrganizationProfileDto>.Of400(
                "Invalid state value",
                new List<Validation400Error>
                {
                    new()
                    {
                        Field = "state",
                        Message = $"'{request.State}' is not a valid Australian state",
                    },
                }
            );
        }

        organization.CompanyName = request.CompanyName;
        organization.ABN = request.ABN;
        organization.IndustryType = request.IndustryType;
        organization.ContactEmail = request.ContactEmail;
        organization.PhoneNumber = request.PhoneNumber;
        organization.AddressLine1 = request.AddressLine1;
        organization.AddressLine2 = request.AddressLine2;
        organization.Suburb = request.Suburb;
        organization.State = stateEnum;
        organization.Postcode = request.Postcode;
        organization.LogoUrl = request.LogoUrl;

        if (!string.IsNullOrEmpty(request.PrimaryAward))
        {
            if (
                !Enum.TryParse<AwardType>(request.PrimaryAward, ignoreCase: true, out var awardType)
            )
            {
                return Result<OrganizationProfileDto>.Of400(
                    "Invalid award type",
                    new List<Validation400Error>
                    {
                        new()
                        {
                            Field = "primaryAward",
                            Message = $"'{request.PrimaryAward}' is not a valid award type",
                        },
                    }
                );
            }

            var existing = organization.OrganizationAwards.FirstOrDefault(oa => oa.IsPrimary);
            if (existing != null)
            {
                existing.AwardType = awardType;
                existing.AddedAt = DateTime.UtcNow;
            }
            else
            {
                organization.OrganizationAwards.Add(
                    new OrganizationAward
                    {
                        OrganizationId = organization.Id,
                        AwardType = awardType,
                        IsPrimary = true,
                        AddedAt = DateTime.UtcNow,
                    }
                );
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<OrganizationProfileDto>.Of200(
            "Organization profile updated",
            OrganizationProfileDto.FromEntity(organization)
        );
    }
}
