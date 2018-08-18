using System.Security.Cryptography;

namespace Pericles.Crypto
{
    public static class PasswordHasher
    {
        private const int HashIterations = 83;

        public static byte[] Hash(string password)
        {
            var passwordBytes = password.GetBytes();
            var sha256 = new SHA256Managed();
            for (var i = 0; i < HashIterations; i++)
            {
                passwordBytes = sha256.ComputeHash(passwordBytes);
            }

            return passwordBytes;
        }
    }
}