using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common;
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
        // 1. Get current user to find their organization
        var currentUser = await userRepository.GetByIdAsync(
            request.CurrentUserId,
            cancellationToken
        );
        if (currentUser == null)
            return Result<List<TeamMemberDto>>.Unauthorized("User not found");

        // 2. Get all team members in the same organization
        var teamMembers = await userRepository.GetByOrganizationIdAsync(
            currentUser.OrganizationId,
            cancellationToken
        );

        // 3. Map to DTOs
        var dtos = teamMembers
            .Select(u => new TeamMemberDto
            {
                UserId = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive,
                LastLoginAt = u.LastLoginAt,
            })
            .ToList();

        return Result<List<TeamMemberDto>>.Success(dtos);
    }
}
