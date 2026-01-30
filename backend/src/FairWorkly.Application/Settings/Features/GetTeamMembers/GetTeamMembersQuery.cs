using MediatR;

namespace FairWorkly.Application.Settings.Features.GetTeamMembers;

/// <summary>
/// Query to retrieve all team members for the current user's organization.
/// This is a READ operation - returns list of TeamMemberDto.
/// </summary>
public class GetTeamMembersQuery : IRequest<List<TeamMemberDto>>
{
    /// <summary>
    /// The organization ID to filter users by.
    /// Populated from the authenticated user's claims.
    /// </summary>
    public Guid OrganizationId { get; set; }
}
