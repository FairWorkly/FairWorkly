using MediatR;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Domain.Exceptions;

namespace FairWorkly.Application.Settings.Features.UpdateTeamMember;

public class UpdateTeamMemberHandler
    : IRequestHandler<UpdateTeamMemberCommand, UpdateTeamMemberResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTeamMemberHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateTeamMemberResult> Handle(
        UpdateTeamMemberCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Fetch the user to update
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null || user.IsDeleted)
        {
            throw new NotFoundException(nameof(User), request.UserId);
        }

        // 2. Security check: Verify user belongs to same organization
        if (user.OrganizationId != request.RequestingUserOrganizationId)
        {
            throw new ForbiddenAccessException(
                "Cannot modify users from a different organization");
        }

        // 3. Apply updates (only if values provided)
        if (!string.IsNullOrEmpty(request.Role))
        {
            user.Role = Enum.Parse<UserRole>(request.Role);
        }

        if (request.IsActive.HasValue)
        {
            user.IsActive = request.IsActive.Value;
        }

        // 4. Save changes
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Return updated data
        return new UpdateTeamMemberResult
        {
            Id = user.Id,
            Name = $"{user.FirstName} {user.LastName}".Trim(),
            Email = user.Email,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt
        };
    }
}
