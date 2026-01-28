using System.Text.Json.Serialization;

namespace FairWorkly.Application.Auth.Features.Refresh;

public class RefreshResponse
{
    public string AccessToken { get; set; } = string.Empty;

    // New refresh token plain to be written to cookie by controller
    [JsonIgnore]
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiration { get; set; }
}
