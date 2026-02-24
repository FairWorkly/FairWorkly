using FairWorkly.Application.Auth.Features.Login;
using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Domain.Auth.Entities;
using FairWorkly.Domain.Auth.Enums;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace FairWorkly.Application.Auth.Features.Register;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    IOrganizationRepository organizationRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    ISecretHasher secretHasher,
    IUnitOfWork unitOfWork,
    IConfiguration configuration
) : IRequestHandler<RegisterCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken
    )
    {
        var email = request.Email.Trim();
        var contactEmail = request.ContactEmail.Trim();
        var abn = request.Abn.Trim();
        var companyName = request.CompanyName.Trim();
        var industryType = request.IndustryType.Trim();
        var addressLine1 = request.AddressLine1.Trim();
        var addressLine2 = request.AddressLine2?.Trim();
        var suburb = request.Suburb.Trim();
        var postcode = request.Postcode.Trim();
        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();

        var state = Enum.Parse<AustralianState>(request.State, true);

        if (!await userRepository.IsEmailUniqueAsync(email, cancellationToken))
        {
            return Result<LoginResponse>.Of409("Email already exists.");
        }

        if (!await organizationRepository.IsAbnUniqueAsync(abn, cancellationToken))
        {
            return Result<LoginResponse>.Of409("ABN already exists.");
        }

        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            CompanyName = companyName,
            ABN = abn,
            IndustryType = industryType,
            AddressLine1 = addressLine1,
            AddressLine2 = addressLine2,
            Suburb = suburb,
            State = state,
            Postcode = postcode,
            ContactEmail = contactEmail,
            PhoneNumber = null,
            SubscriptionTier = SubscriptionTier.Tier1,
            SubscriptionStartDate = DateTime.UtcNow,
            IsSubscriptionActive = true,
            CurrentEmployeeCount = 0,
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Role = UserRole.Admin,
            IsActive = true,
            OrganizationId = organization.Id,
            PasswordHash = passwordHasher.Hash(request.Password),
        };

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();
        var refreshTokenHash = secretHasher.Hash(refreshToken);

        user.RefreshToken = refreshTokenHash;
        var refreshTokenDays = configuration.GetValue<int>("JwtSettings:RefreshTokenExpiryDays", 7);
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDays);
        user.RefreshTokenExpiresAt = refreshTokenExpiresAt;
        user.LastLoginAt = DateTime.UtcNow;

        organizationRepository.Add(organization);
        userRepository.Add(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<LoginResponse>.Of201(
            "Registration successful",
            new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenExpiresAt,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.ToString(),
                    OrganizationId = user.OrganizationId,
                },
            }
        );
    }
}
