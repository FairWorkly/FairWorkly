using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Auth.Features.Login;

public class LoginCommand : IRequest<Result<LoginResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
