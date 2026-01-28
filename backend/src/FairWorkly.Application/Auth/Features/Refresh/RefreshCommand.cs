using MediatR;
using FairWorkly.Domain.Common;

namespace FairWorkly.Application.Auth.Features.Refresh;

public class RefreshCommand : IRequest<Result<RefreshResponse>>
{
    // Plain refresh token read from HttpOnly cookie
    public string RefreshTokenPlain { get; set; } = string.Empty;
}
