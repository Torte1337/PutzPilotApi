using System;
using System.Security.Cryptography;
using System.Text;

namespace PutzPilotApi.Manager
{
    public static class PasswordManager
    {
        private const int SaltSize = 16; 
        private const int HashSize = 32;
        private const int Iterations = 10000;

        public static string OnGenerateSalt()
        {
            byte[] saltBytes = new byte[SaltSize];
            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static string OnHashPassword(string password, string salt)
        {
            // Konvertiere das Salt von Base64 zurück in Bytes
            byte[] saltBytes = Convert.FromBase64String(salt);

            // Hashen des Passworts mit PBKDF2
            using(var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = pbkdf2.GetBytes(HashSize);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool OnVerifyPassword(string enteredPassword, string storedSalt, string storedHash)
        {
            // Hashen des eingegebenen Passworts mit dem gespeicherten Salt
            var enteredHash = OnHashPassword(enteredPassword, storedSalt);
            return enteredHash == storedHash;
        }
    }
}
