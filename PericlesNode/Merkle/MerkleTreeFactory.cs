using System.Collections.Generic;
using System.Linq;
using Pericles.Hashing;
using Pericles.Votes;

namespace Pericles.Merkle
{
    public class MerkleTreeFactory
    {
        private readonly MerkleNodeFactory merkleNodeFactory;

        public MerkleTreeFactory(MerkleNodeFactory merkleNodeFactory)
        {
            this.merkleNodeFactory = merkleNodeFactory;
        }

        public MerkleTree BuildMerkleTree(List<Vote> votes)
        {
            var voteHashes = votes.Select(x => x.Hash); 
            var leaves = voteHashes.Select(this.merkleNodeFactory.BuildLeaf).ToList();
            if (!leaves.Any())
            {
                return null;
            }

            var merkleRoot = this.BuildMerkleRoot(leaves);
            var leafNodesDictionary = this.BuildLeafNodesDictionary(leaves);
            return new MerkleTree(merkleRoot, votes, leafNodesDictionary);
        }

        private MerkleNode BuildMerkleRoot(List<MerkleNode> nodeList)
        {
            while (nodeList.Count > 1)
            {
                var size = nodeList.Count;
                if (size % 2 != 0)
                {
                    nodeList.Add(nodeList.Last()); // duplicate last entry
                }

                nodeList = this.HashNodesPairwise(nodeList);
            }

            return nodeList[0];
        }

        private List<MerkleNode> HashNodesPairwise(List<MerkleNode> previousLevelOfTree)
        {
            var inputSize = previousLevelOfTree.Count;
            var newNodes = new List<MerkleNode>(inputSize / 2);
            for (var i = 0; i < inputSize; i += 2)
            {
                var leftChild = previousLevelOfTree[i];
                var rightChild = previousLevelOfTree[i + 1];
                var newNode = this.merkleNodeFactory.BuildInternalNode(leftChild, rightChild);
                newNodes.Add(newNode);
            }

            return newNodes;
        }

        private Dictionary<Hash, MerkleNode> BuildLeafNodesDictionary(List<MerkleNode> leafNodes)
        {
            var leafNodesDictionary = new Dictionary<Hash, MerkleNode>();
            foreach (var leafNode in leafNodes)
            {
                leafNodesDictionary[leafNode.Hash] = leafNode;
            }

            return leafNodesDictionary;
        }
    }
}