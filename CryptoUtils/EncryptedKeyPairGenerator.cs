using System.Security.Cryptography;

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
            var hashedPasswordBytes = PasswordHasher.Hash(password);

            return new EncryptedKeyPair(publicKeyStringBytes, privateKeyStringBytes, encryptedPrivateKeyStringBytes, iv, hashedPasswordBytes);
        }
    }
}