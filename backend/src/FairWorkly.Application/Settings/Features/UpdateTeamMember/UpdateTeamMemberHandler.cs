using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Settings.Features.UpdateTeamMember;

public class UpdateTeamMemberHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateTeamMemberCommand, Result<TeamMemberUpdatedDto>>
{
    public async Task<Result<TeamMemberUpdatedDto>> Handle(
        UpdateTeamMemberCommand request,
        CancellationToken cancellationToken
    )
    {
        // 1. Get target user to update
        var targetUser = await userRepository.GetByIdAsync(request.TargetUserId, cancellationToken);
        if (targetUser == null)
            return Result<TeamMemberUpdatedDto>.Of404($"User {request.TargetUserId} not found");

        // 2. Verify same organization (multi-tenancy security)
        if (targetUser.OrganizationId != request.OrganizationId)
            return Result<TeamMemberUpdatedDto>.Of403(
                "Cannot modify users from other organizations"
            );

        // 3. Update fields if provided
        if (request.Role != null)
        {
            // Role format already validated by FluentValidation (UpdateTeamMemberValidator)
            var parsedRole = Enum.Parse<UserRole>(request.Role, ignoreCase: true);

            // Prevent self-demotion (Admin can't remove their own admin role)
            if (request.CurrentUserId == request.TargetUserId && parsedRole != UserRole.Admin)
                return Result<TeamMemberUpdatedDto>.Of409("Cannot demote yourself from Admin role");

            targetUser.Role = parsedRole;
        }

        if (request.IsActive.HasValue)
        {
            // Prevent self-deactivation
            if (request.CurrentUserId == request.TargetUserId && !request.IsActive.Value)
                return Result<TeamMemberUpdatedDto>.Of409("Cannot deactivate your own account");

            targetUser.IsActive = request.IsActive.Value;
        }

        // 4. Mark for update and persist
        userRepository.Update(targetUser);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Return success with updated values
        return Result<TeamMemberUpdatedDto>.Of200(
            "Team member updated",
            new TeamMemberUpdatedDto
            {
                UserId = targetUser.Id,
                Role = targetUser.Role,
                IsActive = targetUser.IsActive,
            }
        );
    }
}
