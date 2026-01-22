using MediatR;

namespace FairWorkly.Application.Auth.Features.Refresh;

public class RefreshCommand : IRequest<RefreshResult>
{
    // Plain refresh token read from HttpOnly cookie
    public string RefreshTokenPlain { get; set; } = string.Empty;
}
