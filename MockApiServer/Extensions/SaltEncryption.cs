using System.Security.Cryptography;
using System.Text;

namespace MockApiServer.Extensions;

public static class SaltEncryption
{
    public static string HashPassword(string password, string saltKey)
    {
        if (string.IsNullOrEmpty(password)) return string.Empty;

        var keyBytes = Encoding.UTF8.GetBytes(saltKey);
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(passwordBytes);

        return Convert.ToBase64String(hashBytes);
    }

    public static bool VerifyPassword(string inputPassword, string storedHash, string saltKey)
    {
        var hashOfInput = HashPassword(inputPassword, saltKey);
        return hashOfInput == storedHash;
    }
}
