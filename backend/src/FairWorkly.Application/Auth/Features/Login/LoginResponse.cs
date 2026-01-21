using System.Text.Json.Serialization;

namespace FairWorkly.Application.Auth.Features.Login;

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;

    // The refresh token is set as a cookie at the controller layer
    // and excluded from the JSON body to prevent exposure in API responses.
    [JsonIgnore]
    public string RefreshToken { get; set; } = string.Empty;

    // Do not return JSON to the frontend, only write Cookie for the Controller to use
    [JsonIgnore]
    public DateTime RefreshTokenExpiration { get; set; }

    public UserDto User { get; set; } = null!;
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public Guid OrganizationId { get; set; }
}
