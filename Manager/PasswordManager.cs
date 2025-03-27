using System;
using System.Security.Cryptography;
using System.Text;

namespace PutzPilotApi.Manager;

public static class PasswordManager
{
    public static string OnGenerateSalt()
    {
        byte[] saltBytes = new byte[16];
        using(var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }
    public static string OnHashPassword(string password, string salt)
    {
        using var sha256 = SHA256.Create();
        var combined = Encoding.UTF8.GetBytes(password + salt);
        var hash = sha256.ComputeHash(combined);
        return Convert.ToBase64String(hash);
    }
    public static bool OnVerifyPassword(string enteredPassword, string storedSalt,string storedHash)
    {
        var enteredHash = OnHashPassword(enteredPassword,storedSalt);
        return enteredHash == storedHash;
    }
}
