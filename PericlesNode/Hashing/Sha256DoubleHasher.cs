using System.Security.Cryptography;

namespace Pericles.Hashing
{
    public class Sha256DoubleHasher
    {
        private readonly SHA256Managed sha256;
        private readonly object locker;

        public Sha256DoubleHasher()
        {
            this.sha256 = new SHA256Managed();
            this.locker = new object();
        }

        public Hash DoubleHash(byte[] bytes)
        {
            lock (this.locker)
            {
                var hash = this.sha256.ComputeHash(this.sha256.ComputeHash(bytes));
                return new Hash(hash);
            }
        }
    }
}
