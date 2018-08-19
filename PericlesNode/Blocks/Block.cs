using System.Text;
using Pericles.Hashing;
using Pericles.Merkle;

namespace Pericles.Blocks
{
    public class Block
    {
        public const int MaxVotes = 10;
        public const uint MagicNumber = 0xD9B4BEF9;

        public Block(BlockHeader header, MerkleTree merkleTree, string minerId)
        {
            this.Header = header;
            this.MerkleTree = merkleTree;
            this.VoteCounter = merkleTree.LeafNodesDictionary.Count;
            this.MinerId = minerId;
            this.Hash = this.ComputeHash();
        }

        public BlockHeader Header { get; }
        public MerkleTree MerkleTree { get; }
        public int VoteCounter { get; }
        public string MinerId { get; }
        public Hash Hash { get; private set; }

        public void IncrementNonce()
        {
            this.Header.IncrementNonce();
            this.Hash = this.ComputeHash();
        }

        public override string ToString()
        {
            var transactions = this.MerkleTree.Votes;
            var sb = new StringBuilder();
            for (var i = 0; i < transactions.Count; i++)
            {
                var vote = transactions[i];
                sb.AppendLine($"    vote {i} -- {vote}");
            }

            return $"hash: [{this.Hash}]\nprevBlockHash: [{this.Header.PrevBlockHash}]\nmerkleRootHash:[{this.Header.MerkleRootHash}]\n{sb}";
        }

        private Hash ComputeHash()
        {
            return new Sha256DoubleHasher().DoubleHash(this.Header.GetBytes());
        }
    }
}
