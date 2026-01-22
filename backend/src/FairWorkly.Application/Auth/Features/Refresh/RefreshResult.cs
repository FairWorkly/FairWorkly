namespace FairWorkly.Application.Auth.Features.Refresh;

public class RefreshResult
{
    public RefreshResponse? Response { get; set; }
    public RefreshFailureReason? FailureReason { get; set; }
}

public enum RefreshFailureReason
{
    InvalidToken,
    ExpiredToken,
    AccountDisabled
}
