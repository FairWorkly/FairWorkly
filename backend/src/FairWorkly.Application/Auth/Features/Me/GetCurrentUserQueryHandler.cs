using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Application.Auth.Features.Login;
using MediatR;

namespace FairWorkly.Application.Auth.Features.Me;

public class GetCurrentUserQueryHandler(
    IUserRepository userRepository
) : IRequestHandler<GetCurrentUserQuery, UserDto?>
{
    public async Task<UserDto?> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString(),
            OrganizationId = user.OrganizationId
        };
    }
}
