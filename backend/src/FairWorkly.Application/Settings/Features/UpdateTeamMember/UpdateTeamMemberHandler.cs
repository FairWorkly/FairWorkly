using MediatR;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Application.Common.Interfaces;

namespace FairWorkly.Application.Settings.Features.UpdateTeamMember;

public class UpdateTeamMemberHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<UpdateTeamMemberCommand, Result<TeamMemberUpdatedDto>>
{
    public async Task<Result<TeamMemberUpdatedDto>> Handle(
        UpdateTeamMemberCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Get current user (the admin making the request)
        var currentUser = await userRepository.GetByIdAsync(request.CurrentUserId, cancellationToken);
        if (currentUser == null)
            return Result<TeamMemberUpdatedDto>.Unauthorized("Current user not found");

        // 2. Get target user to update
        var targetUser = await userRepository.GetByIdAsync(request.TargetUserId, cancellationToken);
        if (targetUser == null)
            return Result<TeamMemberUpdatedDto>.NotFound($"User {request.TargetUserId} not found");

        // 3. Verify same organization (multi-tenancy security)
        if (targetUser.OrganizationId != currentUser.OrganizationId)
            return Result<TeamMemberUpdatedDto>.Forbidden("Cannot modify users from other organizations");

        // 4. Prevent self-demotion (Admin can't remove their own admin role)
        if (request.CurrentUserId == request.TargetUserId && request.Role == "Manager")
            return Result<TeamMemberUpdatedDto>.Failure("Cannot demote yourself from Admin role");

        // 5. Update fields if provided
        if (request.Role != null)
        {
            targetUser.Role = Enum.Parse<UserRole>(request.Role);
        }

        if (request.IsActive.HasValue)
        {
            // Prevent self-deactivation
            if (request.CurrentUserId == request.TargetUserId && !request.IsActive.Value)
                return Result<TeamMemberUpdatedDto>.Failure("Cannot deactivate your own account");

            targetUser.IsActive = request.IsActive.Value;
        }

        // 6. Mark for update and persist
        userRepository.Update(targetUser);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Return success with updated values
        return Result<TeamMemberUpdatedDto>.Success(new TeamMemberUpdatedDto
        {
            Id = targetUser.Id,
            Role = targetUser.Role.ToString(),
            IsActive = targetUser.IsActive
        });
    }
}
