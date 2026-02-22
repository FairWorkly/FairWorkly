using FairWorkly.Application.Auth.Features.Login;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Auth.Features.Me;

public class GetCurrentUserQuery : IRequest<Result<UserDto>>
{
    public Guid UserId { get; set; }
}
