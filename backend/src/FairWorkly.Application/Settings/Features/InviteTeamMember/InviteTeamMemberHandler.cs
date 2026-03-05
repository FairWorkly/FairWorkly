using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Settings.Helpers;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FairWorkly.Application.Settings.Features.InviteTeamMember;

public class InviteTeamMemberHandler(
    IUserRepository userRepository,
    ISecretHasher secretHasher,
    IUnitOfWork unitOfWork,
    IConfiguration configuration,
    ILogger<InviteTeamMemberHandler> logger
) : IRequestHandler<InviteTeamMemberCommand, Result<InviteTeamMemberResponse>>
{
    public async Task<Result<InviteTeamMemberResponse>> Handle(
        InviteTeamMemberCommand request,
        CancellationToken cancellationToken
    )
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();
        if (
            !Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role)
            || role == UserRole.Unknown
        )
        {
            return Result<InviteTeamMemberResponse>.Of409(
                "Invalid role. Must be Admin or Manager."
            );
        }

        // 1. Check email uniqueness within organization
        var isUnique = await userRepository.IsEmailUniqueAsync(
            request.OrganizationId,
            email,
            cancellationToken
        );
        if (!isUnique)
        {
            return Result<InviteTeamMemberResponse>.Of409(
                "A user with this email already exists in your organization."
            );
        }

        // 2. Generate secure invitation token
        var (plainToken, tokenHash, expiresAt) = InvitationHelper.GenerateToken(
            secretHasher,
            configuration
        );

        // 3. Create user with pending invitation status
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            OrganizationId = request.OrganizationId,
            IsActive = false,
            CreatedByUserId = request.InvitedByUserId,
        };
        user.SetInvitationToken(tokenHash, expiresAt);
        user.ValidateDomainRules();

        userRepository.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Build invite link
        var inviteLink = InvitationHelper.BuildInviteLink(configuration, plainToken);

        logger.LogInformation(
            "Team member invited: {Email} to organization {OrgId}",
            email,
            request.OrganizationId
        );

        return Result<InviteTeamMemberResponse>.Of201(
            "Team member invited successfully",
            new InviteTeamMemberResponse { UserId = user.Id, InviteLink = inviteLink }
        );
    }
}
