using System;
using System.IO;
using System.Security.Cryptography;

namespace Pericles.Crypto
{
    public static class PasswordEncrypter
    {
        private const int Iterations = 1000;
        private static readonly byte[] DummySalt = { 0, 1, 2, 3, 4, 5, 6, 7 };

        public static byte[] EncryptBytes(string password, byte[] unencryptedBytes, out byte[] iv)
        {
            var deriveBytes = new Rfc2898DeriveBytes(password, DummySalt, Iterations);
            var tripleDes = TripleDES.Create();
            tripleDes.Key = deriveBytes.GetBytes(16);

            using (var memStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memStream, tripleDes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(unencryptedBytes, 0, unencryptedBytes.Length);
                cryptoStream.FlushFinalBlock();
                cryptoStream.Close();
                iv = tripleDes.IV;
                return memStream.ToArray();
            }
        }

        public static byte[] DecryptBytes(string password, byte[] encryptedBytes, byte[] iv)
        {
            var deriveBytes = new Rfc2898DeriveBytes(password, DummySalt, Iterations);
            var tripleDes = TripleDES.Create();
            tripleDes.Key = deriveBytes.GetBytes(16);
            tripleDes.IV = iv;

            using (var memStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memStream, tripleDes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                cryptoStream.Flush();
                cryptoStream.Close();
                return memStream.ToArray();
            }
        }
    }
}