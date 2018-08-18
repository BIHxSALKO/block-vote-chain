namespace Pericles.Crypto
{
    public class EncryptedKeyPair
    {
        public EncryptedKeyPair(
            byte[] publicKey,
            byte[] privateKey,
            byte[] encryptedPrivateKey,
            byte[] initializationVector,
            byte[] hashedPassword)
        {
            this.PublicKey = publicKey;
            this.PrivateKey = privateKey;
            this.EncryptedPrivateKey = encryptedPrivateKey;
            this.InitializationVector = initializationVector;
            this.HashedPassword = hashedPassword;
        }

        public byte[] PublicKey { get; }
        public byte[] PrivateKey { get; }
        public byte[] EncryptedPrivateKey { get; }
        public byte[] InitializationVector { get; }
        public byte[] HashedPassword { get; }
    }
}