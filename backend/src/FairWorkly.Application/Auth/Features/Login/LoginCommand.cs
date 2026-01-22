using MediatR;

namespace FairWorkly.Application.Auth.Features.Login;

public class LoginCommand : IRequest<LoginResult>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
