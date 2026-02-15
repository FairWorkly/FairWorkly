using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Auth.Features.ForgotPassword;

public class ForgotPasswordCommand : IRequest<Result<bool>>
{
    public string Email { get; set; } = string.Empty;
}
