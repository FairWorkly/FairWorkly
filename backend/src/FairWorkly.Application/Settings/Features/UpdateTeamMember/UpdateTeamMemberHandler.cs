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
        // 1. Get current user (the admin making the request)
        var currentUser = await userRepository.GetByIdAsync(
            request.CurrentUserId,
            cancellationToken
        );
        if (currentUser == null)
            return Result<TeamMemberUpdatedDto>.Of401("Current user not found");

        // 2. Get target user to update
        var targetUser = await userRepository.GetByIdAsync(request.TargetUserId, cancellationToken);
        if (targetUser == null)
            return Result<TeamMemberUpdatedDto>.Of404($"User {request.TargetUserId} not found");

        // 3. Verify same organization (multi-tenancy security)
        if (targetUser.OrganizationId != currentUser.OrganizationId)
            return Result<TeamMemberUpdatedDto>.Of403(
                "Cannot modify users from other organizations"
            );

        // 4. Update fields if provided
        if (request.Role != null)
        {
            if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var parsedRole))
                return Result<TeamMemberUpdatedDto>.Of400(
                    "Invalid role",
                    new List<Validation400Error>
                    {
                        new() { Field = "role", Message = $"Invalid role: {request.Role}" },
                    }
                );
            // Prevent self-demotion (Admin can't remove their own admin role)
            if (request.CurrentUserId == request.TargetUserId && parsedRole != UserRole.Admin)
                return Result<TeamMemberUpdatedDto>.Of400(
                    "Cannot demote yourself",
                    new List<Validation400Error>
                    {
                        new()
                        {
                            Field = "role",
                            Message = "Cannot demote yourself from Admin role",
                        },
                    }
                );

            targetUser.Role = parsedRole;
        }

        if (request.IsActive.HasValue)
        {
            // Prevent self-deactivation
            if (request.CurrentUserId == request.TargetUserId && !request.IsActive.Value)
                return Result<TeamMemberUpdatedDto>.Of400(
                    "Cannot deactivate self",
                    new List<Validation400Error>
                    {
                        new()
                        {
                            Field = "isActive",
                            Message = "Cannot deactivate your own account",
                        },
                    }
                );

            targetUser.IsActive = request.IsActive.Value;
        }

        // 5. Mark for update and persist
        userRepository.Update(targetUser);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. Return success with updated values
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
