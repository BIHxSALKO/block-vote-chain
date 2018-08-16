using System.Text;
using Pericles.Hashing;
using Pericles.Merkle;

namespace Pericles.Blocks
{
    public class Block
    {
        public const int MaxNonCoinbaseTransactions = 9;
        public const uint MagicNumber = 0xD9B4BEF9;

        public Block(BlockHeader header, MerkleTree merkleTree)
        {
            this.Header = header;
            this.MerkleTree = merkleTree;
            this.TransactionCounter = merkleTree.LeafNodesDictionary.Count;
            this.Hash = this.ComputeHash();
        }

        public BlockHeader Header { get; }
        public MerkleTree MerkleTree { get; }
        public int TransactionCounter { get; }
        public Hash Hash { get; private set; }

        public void IncrementNonce()
        {
            this.Header.IncrementNonce();
            this.Hash = this.ComputeHash();
        }

        public override string ToString()
        {
            var transactions = this.MerkleTree.Transactions;
            var sb = new StringBuilder();
            for (var i = 0; i < transactions.Count; i++)
            {
                var transaction = transactions[i];
                sb.AppendLine($"    transaction {i} -- {transaction}");
            }

            return $"hash: [{this.Hash}]\nprevBlockHash: [{this.Header.PrevBlockHash}]\nmerkleRootHash:[{this.Header.MerkleRootHash}]\n{sb}";
        }

        private Hash ComputeHash()
        {
            return Sha256DoubleHasher.DoubleHash(this.Header.GetBytes());
        }
    }
}
