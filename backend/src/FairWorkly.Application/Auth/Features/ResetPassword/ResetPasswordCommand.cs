using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Auth.Features.ResetPassword;

public class ResetPasswordCommand : IRequest<Result<bool>>
{
    public string Token { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
