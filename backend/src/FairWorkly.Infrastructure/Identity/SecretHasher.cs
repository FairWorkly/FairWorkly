using System;
using System.Security.Cryptography;
using System.Text;
using FairWorkly.Application.Common.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace FairWorkly.Infrastructure.Identity;

public class SecretHasher : ISecretHasher
{
    public string Hash(string plain)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(plain);
        var hashed = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hashed);
    }

    public bool Verify(string plain, string hash)
    {
        var computed = Hash(plain);
        return ConstantTimeEquals(
            Convert.FromBase64String(computed),
            Convert.FromBase64String(hash)
        );
    }

    public string GenerateToken(int size = 32)
    {
        var buf = new byte[size];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(buf);
        return Base64UrlEncoder.Encode(buf);
    }

    private static bool ConstantTimeEquals(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
            return false;
        int diff = 0;
        for (int i = 0; i < a.Length; i++)
            diff |= a[i] ^ b[i];
        return diff == 0;
    }
}
