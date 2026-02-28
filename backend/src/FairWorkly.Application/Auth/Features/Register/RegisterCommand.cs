using FairWorkly.Application.Auth.Features.Login;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Result;
using MediatR;

namespace FairWorkly.Application.Auth.Features.Register;

public class RegisterCommand : IRequest<Result<LoginResponse>>
{
    public string CompanyName { get; set; } = string.Empty;
    public string Abn { get; set; } = string.Empty;
    public string IndustryType { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string Suburb { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
