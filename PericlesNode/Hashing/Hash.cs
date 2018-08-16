using System;
using System.Linq;
using System.Numerics;
using Google.Protobuf;

namespace Pericles.Hashing
{
    [Serializable]
    public class Hash
    {
        private readonly byte[] bytes;
        
        public Hash(byte[] bytes)
        {
            this.bytes = bytes;
        }

        public Hash(ByteString byteString)
        {
            this.bytes = byteString.ToByteArray();
        }

        public Hash(string hexString)
        {
            this.bytes = HexStringConverter.HexStringToBytes(hexString);
        }

        public byte[] GetBytes()
        {
            return this.bytes;
        }

        public BigInteger ToBigInteger()
        {
            return new BigInteger(this.bytes.Concat(new [] { (byte)0 }).ToArray()); // ensure result is unsigned
        }

        public override int GetHashCode()
        {
            return BitConverter.ToInt32(this.bytes, 0);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Hash))
            {
                return false;
            }

            return this.Equals(obj as Hash);
        }

        public override string ToString()
        {
            return HexStringConverter.BytesToHexString(this.bytes);
        }


        private bool Equals(Hash other)
        {
            return this.bytes.SequenceEqual(other.bytes);
        }
    }
}
