using System;
using NikCoin.Hashing;

namespace NikCoin.Merkle
{
    [Serializable]
    public class MerkleNode
    {
        public MerkleNode(Hash hash)
        {
            this.Hash = hash;
            this.LeftChild = null;
            this.RightChild = null;
            this.Parent = null;
        }

        public MerkleNode(Hash hash, MerkleNode leftChild, MerkleNode rightChild)
        {
            this.Hash = hash;
            this.LeftChild = leftChild;
            this.RightChild = rightChild;
            this.Parent = null;

            this.LeftChild.SetParent(this, true);
            this.RightChild.SetParent(this, false);
        }

        public Hash Hash { get; }
        public MerkleNode LeftChild { get; }
        public MerkleNode RightChild { get; }
        public MerkleNode Parent { get; private set; }
        public bool IsLeftChild { get; private set; }

        public void SetParent(MerkleNode parentNode, bool isLeftChild)
        {
            this.Parent = parentNode;
            this.IsLeftChild = isLeftChild;
        }

        public override string ToString()
        {
            return this.Hash.ToString();
        }
    }
}