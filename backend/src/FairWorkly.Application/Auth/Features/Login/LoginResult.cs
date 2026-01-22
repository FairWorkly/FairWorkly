namespace FairWorkly.Application.Auth.Features.Login;

public class LoginResult
{
    public LoginResponse? Response { get; set; }
    public LoginFailureReason? FailureReason { get; set; }
}

public enum LoginFailureReason
{
    InvalidCredentials,
    AccountDisabled
}
