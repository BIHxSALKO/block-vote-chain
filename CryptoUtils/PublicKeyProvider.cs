using System.Security.Cryptography;
using System.Text;

namespace Pericles.Crypto
{
    public static class PublicKeyProvider
    {
        public static RSACryptoServiceProvider GetPublicKey(byte[] publicKeyBytes)
        {
            var publicKeyString = Encoding.UTF8.GetString(publicKeyBytes);
            var publicKey = new RSACryptoServiceProvider();
            publicKey.FromXmlString(publicKeyString);
            return publicKey;
        }
    }
}