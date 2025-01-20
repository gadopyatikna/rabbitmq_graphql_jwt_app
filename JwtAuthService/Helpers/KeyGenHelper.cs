using System.Security.Cryptography;

namespace JwtAuthService.Helpers;

public class KeyGenHelper
{
    public static string GenerateKey()
    {
        // Generate a random 256-bit key (32 bytes)
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] key = new byte[32];  // 32 bytes = 256 bits
            rng.GetBytes(key);

            return Convert.ToBase64String(key);
        }
    }
}