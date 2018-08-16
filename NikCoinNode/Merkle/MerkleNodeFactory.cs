using System.Linq;
using NikCoin.Hashing;

namespace NikCoin.Merkle
{
    public class MerkleNodeFactory
    {
        public MerkleNode BuildLeaf(Hash hash)
        {
            return new MerkleNode(hash);
        }

        public MerkleNode BuildInternalNode(MerkleNode leftChild, MerkleNode rightChild)
        {
            var concatenatedHashes = leftChild.Hash.GetBytes().Concat(rightChild.Hash.GetBytes()).ToArray();
            var hash = Sha256DoubleHasher.DoubleHash(concatenatedHashes);
            return new MerkleNode(hash, leftChild, rightChild);
        }
    }
}