using FairWorkly.Application.Awards.Interfaces;
using FairWorkly.Domain.Common;
using MediatR;

namespace FairWorkly.Application.Awards.Features.GetOrganizationAwards;

public class GetOrganizationAwardsHandler(IAwardRepository awardRepository)
    : IRequestHandler<GetOrganizationAwardsQuery, Result<GetOrganizationAwardsResponse>>
{
    public async Task<Result<GetOrganizationAwardsResponse>> Handle(
        GetOrganizationAwardsQuery request,
        CancellationToken cancellationToken
    )
    {
        var awards = await awardRepository.GetByOrganizationIdAsync(
            request.OrganizationId,
            cancellationToken
        );

        var response = new GetOrganizationAwardsResponse
        {
            Awards = awards
                .Select(a => new OrganizationAwardItem
                {
                    AwardType = a.AwardType.ToString(),
                    Name = a.Name,
                    AwardCode = a.AwardCode,
                    Description = a.Description,
                    IsPrimary = a.IsPrimary,
                    EmployeeCount = a.EmployeeCount,
                })
                .ToList(),
        };

        return Result<GetOrganizationAwardsResponse>.Success(response);
    }
}
