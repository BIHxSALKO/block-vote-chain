using Pericles.Hashing;
using Pericles.Merkle;
using Pericles.Votes;

namespace Pericles.Blocks
{
    public static class GenesisBlock
    {
        private static readonly Hash GenesisPrevBlockHash = new Hash(new byte[32]);
        private static readonly MerkleNodeFactory GenesisMerkleNodeFactory = new MerkleNodeFactory();
        private static readonly MerkleTreeFactory GenesisMerkleTreeFactory =
            new MerkleTreeFactory(GenesisMerkleNodeFactory);
        private static readonly BlockFactory GenesisBlockFactory = new BlockFactory(GenesisMerkleTreeFactory);

        public static readonly Block Instance =
            GenesisBlockFactory.Build(GenesisPrevBlockHash, GenesisVote.Instance);
    }
}
