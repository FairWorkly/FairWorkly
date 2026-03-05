using FairWorkly.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FairWorkly.Application.Settings.Helpers;

public static class InvitationHelper
{
    /// <summary>
    /// Generates a new invitation token and returns both plain + hashed values plus expiry.
    /// </summary>
    public static (string PlainToken, string TokenHash, DateTime ExpiresAtUtc) GenerateToken(
        ISecretHasher secretHasher,
        IConfiguration configuration
    )
    {
        var plainToken = secretHasher.GenerateToken(32);
        var tokenHash = secretHasher.Hash(plainToken);
        var expiryDays = configuration.GetValue<int>("AuthSettings:InvitationTokenExpiryDays", 7);
        var expiresAt = DateTime.UtcNow.AddDays(expiryDays);

        return (plainToken, tokenHash, expiresAt);
    }

    /// <summary>
    /// Builds the frontend invite link from a plain token.
    /// </summary>
    public static string BuildInviteLink(IConfiguration configuration, string plainToken)
    {
        var baseUrl = configuration.GetValue<string>("Frontend:BaseUrl") ?? "http://localhost:5173";
        return $"{baseUrl.TrimEnd('/')}/accept-invite?token={plainToken}";
    }
}
