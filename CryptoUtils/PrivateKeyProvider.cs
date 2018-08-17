using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Pericles.Crypto
{
    public static class PrivateKeyProvider
    {
        public static bool TryGetPrivateKey(byte[] encryptedPrivateKeyBytes, string password, byte[] iv, out RSACryptoServiceProvider privateKey)
        {
            var decryptedPrivateKeyBytes = PasswordEncrypter.DecryptBytes(password, encryptedPrivateKeyBytes, iv);
            var decryptedPrivateKey = Encoding.UTF8.GetString(decryptedPrivateKeyBytes);

            privateKey = new RSACryptoServiceProvider();
            try
            {
                privateKey.FromXmlString(decryptedPrivateKey);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}