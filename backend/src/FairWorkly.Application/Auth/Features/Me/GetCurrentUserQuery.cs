using MediatR;
using FairWorkly.Domain.Common;
using FairWorkly.Application.Auth.Features.Login;

namespace FairWorkly.Application.Auth.Features.Me;

public class GetCurrentUserQuery : IRequest<Result<UserDto>>
{
    public Guid UserId { get; set; }
}
