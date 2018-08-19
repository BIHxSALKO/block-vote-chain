using System;
using System.Collections.Generic;
using Pericles.Hashing;
using Pericles.Transactions;
using Pericles.Votes;

namespace Pericles.Merkle
{
    [Serializable]
    public class MerkleTree
    {
        public MerkleTree(
            MerkleNode root, 
            List<Vote> votes, 
            Dictionary<Hash, MerkleNode> leafNodesDictionary)
        {
            this.Root = root;
            this.Votes = votes;
            this.LeafNodesDictionary = leafNodesDictionary;
        }

        public MerkleNode Root { get; }
        public List<Vote> Votes { get; }
        public Dictionary<Hash, MerkleNode> LeafNodesDictionary { get; }

        public override string ToString()
        {
            return this.Root.ToString();
        }
    }
}