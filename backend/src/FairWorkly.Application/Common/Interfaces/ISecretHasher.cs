using System;

namespace FairWorkly.Application.Common.Interfaces
{
    public interface ISecretHasher
    {
        // Hash a high-entropy secret (e.g., refresh token) and return Base64 string
        string Hash(string plain);

        // Verify a plain secret against stored hash using constant-time comparison
        bool Verify(string plain, string hash);

        // Generate a high-entropy random token (Base64Url encoded)
        string GenerateToken(int size = 32);
    }
}
