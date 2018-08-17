namespace Pericles.Crypto
{
    public class EncryptedKeyPair
    {
        public EncryptedKeyPair(
            byte[] publicKey,
            byte[] privateKey,
            byte[] encryptedPrivateKey,
            byte[] initializationVector)
        {
            this.PublicKey = publicKey;
            this.PrivateKey = privateKey;
            this.EncryptedPrivateKey = encryptedPrivateKey;
            this.InitializationVector = initializationVector;
        }

        public byte[] PublicKey { get; }
        public byte[] PrivateKey { get; }
        public byte[] EncryptedPrivateKey { get; }
        public byte[] InitializationVector { get; }
    }
}