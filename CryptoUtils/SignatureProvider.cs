using System.Security.Cryptography;

namespace Pericles.Crypto
{
    public static class SignatureProvider
    {
        public static byte[] Sign(string password, EncryptedKeyPair encryptedKeyPair, byte[] data)
        {
            RSACryptoServiceProvider privateKey;
            PrivateKeyProvider.TryGetPrivateKey(
                encryptedKeyPair.EncryptedPrivateKey,
                password,
                encryptedKeyPair.InitializationVector,
                out privateKey);
            var signature = privateKey.SignData(data, new SHA256CryptoServiceProvider());
            return signature;
        }
    }
}