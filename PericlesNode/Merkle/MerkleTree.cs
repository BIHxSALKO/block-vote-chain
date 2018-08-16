using System;
using System.Collections.Generic;
using Pericles.Hashing;
using Pericles.Transactions;

namespace Pericles.Merkle
{
    [Serializable]
    public class MerkleTree
    {
        public MerkleTree(
            MerkleNode root, 
            List<Transaction> transactions, 
            Dictionary<Hash, MerkleNode> leafNodesDictionary)
        {
            this.Root = root;
            this.Transactions = transactions;
            this.LeafNodesDictionary = leafNodesDictionary;
        }

        public MerkleNode Root { get; }
        public List<Transaction> Transactions { get; }
        public Dictionary<Hash, MerkleNode> LeafNodesDictionary { get; }

        public override string ToString()
        {
            return this.Root.ToString();
        }
    }
}