using MediatR;

namespace FairWorkly.Application.Settings.Features.UpdateTeamMember;

/// <summary>
/// Command to update a team member's role and/or status.
/// This is a WRITE operation - returns the updated member data.
/// </summary>
public class UpdateTeamMemberCommand : IRequest<UpdateTeamMemberResult>
{
    /// <summary>
    /// The user ID to update.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// New role (optional). Must be "Admin" or "Manager".
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// New active status (optional).
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// The organization ID of the requesting user.
    /// Used to verify the target user belongs to same org.
    /// </summary>
    public Guid RequestingUserOrganizationId { get; set; }
}
