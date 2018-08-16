using System.Security.Cryptography;

namespace NikCoin.Hashing
{
    public static class Sha256DoubleHasher
    {
        private static readonly object locker = new object();
        private static readonly SHA256Managed SHA256 = new SHA256Managed();

        public static Hash DoubleHash(byte[] bytes)
        {
            lock (locker)
            {
                var hash = SHA256.ComputeHash(SHA256.ComputeHash(bytes));
                return new Hash(hash);
            }
        }
    }
}
