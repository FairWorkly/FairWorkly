using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Auth.Features.ValidateResetPasswordToken;

public class ValidateResetPasswordTokenQuery : IRequest<Result<bool>>
{
    public string Token { get; set; } = string.Empty;
}
