using MediatR;

namespace FairWorkly.Application.Auth.Features.Logout;

public class LogoutCommand : IRequest<bool>
{
    // Optionally provided: user id from access token
    public Guid? UserId { get; set; }

    // Optionally provided: plain refresh token read from cookie
    public string? RefreshTokenPlain { get; set; }
}
