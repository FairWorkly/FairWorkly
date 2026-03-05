using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Settings.Features.GetTeamMembers;

public class GetTeamMembersQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetTeamMembersQuery, Result<List<TeamMemberDto>>>
{
    public async Task<Result<List<TeamMemberDto>>> Handle(
        GetTeamMembersQuery request,
        CancellationToken cancellationToken
    )
    {
        // 1. Get all team members in the organization
        var teamMembers = await userRepository.GetByOrganizationIdAsync(
            request.OrganizationId,
            cancellationToken
        );

        // 2. Map to DTOs
        var dtos = teamMembers
            .Select(u => new TeamMemberDto
            {
                UserId = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive,
                InvitationStatus = u.InvitationStatus,
                InvitationTokenExpiry = u.InvitationTokenExpiry,
                LastLoginAt = u.LastLoginAt,
            })
            .ToList();

        return Result<List<TeamMemberDto>>.Of200("Team members retrieved", dtos);
    }
}
