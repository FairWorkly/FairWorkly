using MediatR;
using FairWorkly.Domain.Auth.Interfaces;

namespace FairWorkly.Application.Settings.Features.GetTeamMembers;

public class GetTeamMembersHandler
    : IRequestHandler<GetTeamMembersQuery, List<TeamMemberDto>>
{
    private readonly IUserRepository _userRepository;

    public GetTeamMembersHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<TeamMemberDto>> Handle(
        GetTeamMembersQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Query users by organization (multi-tenancy filter)
        var users = await _userRepository.GetByOrganizationIdAsync(
            request.OrganizationId,
            cancellationToken);

        // 2. Map Entity -> DTO
        // Filter out soft-deleted users (IsDeleted == true) - Repository already does this, but keeping it robust
        return users
            .Where(u => !u.IsDeleted)
            .Select(u => new TeamMemberDto
            {
                Id = u.Id,
                Name = $"{u.FirstName} {u.LastName}".Trim(),
                Email = u.Email,
                Role = u.Role.ToString(),
                IsActive = u.IsActive,
                LastLoginAt = u.LastLoginAt
            })
            .OrderBy(u => u.Name)
            .ToList();
    }
}
