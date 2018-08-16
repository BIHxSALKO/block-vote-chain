using System;
using System.Linq;
using Pericles.Hashing;
using Pericles.Utils;

namespace Pericles.Blocks
{
    public class BlockHeader
    {
        public const uint DefaultBits = 0x1e200000;

        public BlockHeader(Hash prevBlockHash, Hash merkleRootHash)
        {
            this.PrevBlockHash = prevBlockHash;
            this.MerkleRootHash = merkleRootHash;
            this.Timestamp = UnixTimestampGenerator.GetUnixTimestamp();
            this.Bits = DefaultBits;
            this.Nonce = 0;
        }

        public BlockHeader(Protocol.BlockHeader protoBlockHeader)
        {
            this.PrevBlockHash = new Hash(protoBlockHeader.PrevBlockHash);
            this.MerkleRootHash = new Hash(protoBlockHeader.MerkleRootHash);
            this.Timestamp = protoBlockHeader.Timestamp;
            this.Bits = protoBlockHeader.Bits;
            this.Nonce = protoBlockHeader.Nonce;
        }

        public Hash PrevBlockHash { get; }
        public Hash MerkleRootHash { get; }
        public double Timestamp { get; private set; }
        public uint Bits { get; }
        public uint Nonce { get; private set; }

        public void IncrementNonce()
        {
            this.Nonce++;
            this.Timestamp = UnixTimestampGenerator.GetUnixTimestamp();
        }

        public byte[] GetBytes()
        {
            return this.PrevBlockHash.GetBytes()
                .Concat(this.MerkleRootHash.GetBytes())
                .Concat(BitConverter.GetBytes(this.Timestamp))
                .Concat(BitConverter.GetBytes(this.Bits))
                .Concat(BitConverter.GetBytes(this.Nonce))
                .ToArray();
        }
    }
}
