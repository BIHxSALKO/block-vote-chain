using System.Security.Cryptography;
using System.Text;

namespace Pericles.Crypto
{
    public static class EncryptedKeyPairGenerator
    {
        public static EncryptedKeyPair Generate(string password)
        {
            var rsaCryptoProvider = new RSACryptoServiceProvider();
            var publicKeyString = rsaCryptoProvider.ToXmlString(false);
            var privateKeyString = rsaCryptoProvider.ToXmlString(true);

            var publicKeyStringBytes = publicKeyString.GetBytes();
            var privateKeyStringBytes = privateKeyString.GetBytes();

            byte[] iv;
            var encryptedPrivateKeyStringBytes = PasswordEncrypter.EncryptBytes(password, privateKeyStringBytes, out iv);
            return new EncryptedKeyPair(publicKeyStringBytes, privateKeyStringBytes, encryptedPrivateKeyStringBytes, iv);
        }
    }
}