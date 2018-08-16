using System.Collections.Generic;

namespace NikCoin.Merkle
{
    public static class MerklePathFinder
    {
        public static List<MerkleNode> FindMerklePath(MerkleTree merkleTree, MerkleNode startNode)
        {
            var merklePath = new List<MerkleNode>();
            var currNode = startNode;
            while (currNode.Parent != null)
            {
                var sibling = GetSibling(currNode);
                merklePath.Add(sibling);
                currNode = currNode.Parent;
            }

            return merklePath;
        }

        private static MerkleNode GetSibling(MerkleNode merkleNode)
        {
            var parent = merkleNode.Parent;
            if (parent == null)
            {
                return null;
            }

            var sibling = !parent.LeftChild.Equals(merkleNode) ? parent.LeftChild : parent.RightChild;
            return sibling;
        }
    }
}