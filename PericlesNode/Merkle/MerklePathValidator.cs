using System.Collections.Generic;
using System.Linq;
using Pericles.Votes;


namespace Pericles.Merkle
{
    public class MerklePathValidator
    {
        private readonly MerkleNodeFactory merkleNodeFactory;

        public MerklePathValidator(MerkleNodeFactory merkleNodeFactory)
        {
            this.merkleNodeFactory = merkleNodeFactory;
        }

        public bool IsVoteInMerkleTree(Vote vote, MerkleTree merkleTree)
        {
            MerkleNode voteNode;
            if (!merkleTree.LeafNodesDictionary.TryGetValue(vote.Hash, out voteNode))
            {
                return false;
            }

            var merklePath = MerklePathFinder.FindMerklePath(merkleTree, voteNode);
            if (!merklePath.Any())
            {
                return voteNode.Hash.Equals(merkleTree.Root.Hash);
            }

            var validationRoot = this.ComputeRootUsingMerklePath(merklePath, voteNode);
            return validationRoot.Hash.Equals(merkleTree.Root.Hash);
        }

        private MerkleNode ComputeRootUsingMerklePath(List<MerkleNode> merklePath, MerkleNode startNode)
        {
            var currNode = startNode;
            foreach (var nextMerklePathNode in merklePath)
            {
                currNode = nextMerklePathNode.IsLeftChild
                    ? this.merkleNodeFactory.BuildInternalNode(nextMerklePathNode, currNode)
                    : this.merkleNodeFactory.BuildInternalNode(currNode, nextMerklePathNode);
            }

            return currNode;
        }
    }
}