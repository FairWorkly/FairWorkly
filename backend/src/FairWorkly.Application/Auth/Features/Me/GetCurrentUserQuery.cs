using MediatR;
using FairWorkly.Application.Auth.Features.Login;

namespace FairWorkly.Application.Auth.Features.Me;

public class GetCurrentUserQuery : IRequest<UserDto?>
{
    public Guid UserId { get; set; }
}
