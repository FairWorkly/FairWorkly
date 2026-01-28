using FairWorkly.Application.Auth.Features.Login;
using FairWorkly.Domain.Auth.Interfaces;
using FairWorkly.Domain.Common;
using MediatR;

namespace FairWorkly.Application.Auth.Features.Me;

public class GetCurrentUserQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken
    )
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result<UserDto>.NotFound("User not found.");
        }

        return Result<UserDto>.Success(
            new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                OrganizationId = user.OrganizationId,
            }
        );
    }
}
