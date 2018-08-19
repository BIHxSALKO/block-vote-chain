using System.Collections.Generic;
using System.Linq;
using Pericles.Hashing;
using Pericles.Merkle;
using Pericles.Votes;

namespace Pericles.Blocks
{
    public class BlockFactory
    {
        private readonly MerkleTreeFactory merkleTreeFactory;
        private readonly string minerId;

        public BlockFactory(MerkleTreeFactory merkleTreeFactory, string minerId)
        {
            this.merkleTreeFactory = merkleTreeFactory;
            this.minerId = minerId;
        }

        public Block Build(Hash prevBlockHash, List<Vote> votes)
        {
            var merkleTree = this.merkleTreeFactory.BuildMerkleTree(votes);
            var blockHeader = new BlockHeader(prevBlockHash, merkleTree.Root.Hash);
            return new Block(blockHeader, merkleTree, this.minerId);
        }

        public Block Build(Protocol.Block protoBlock)
        {
            var votes = protoBlock.Votes.Select(x => new Vote(x)).ToList();
            var merkleTree = this.merkleTreeFactory.BuildMerkleTree(votes);
            var blockHeader = new BlockHeader(protoBlock.BlockHeader);
            return new Block(blockHeader, merkleTree, protoBlock.MinerId);
        }
    }
}