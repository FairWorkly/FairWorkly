using MediatR;

namespace FairWorkly.Application.Auth.Features.Me;

public class GetCurrentUserQuery : IRequest<FairWorkly.Application.Auth.Features.Login.UserDto?>
{
    public Guid UserId { get; set; }
}
