using System;
using System.Collections.Generic;
using Pericles.Votes;

namespace Pericles.Blocks
{
    public class BlockchainAdder
    {
        private readonly Blockchain blockchain;
        private readonly VoteMemoryPool voteMemoryPool;
        private readonly BlockForwarder blockForwarder;

        public BlockchainAdder(
            Blockchain blockchain,
            VoteMemoryPool voteMemoryPool,
            BlockForwarder blockForwarder)
        {
            this.blockchain = blockchain;
            this.voteMemoryPool = voteMemoryPool;
            this.blockForwarder = blockForwarder;
        }

        public void AddNewBlock(Block block)
        {
            this.blockchain.AddBlock(block);
            this.RemoveVotesFromMemPool(block.MerkleTree.Votes);

            Console.WriteLine($"added new block: {block.Hash}");
            Console.WriteLine($"new blockchain height = {this.blockchain.CurrentHeight}");

            this.blockForwarder.ForwardBlock(block);
        }

        private void RemoveVotesFromMemPool(IEnumerable<Vote> votes)
        {
            foreach (var vote in votes)
            {
                this.voteMemoryPool.DeleteVote(vote.Hash);
            }
        }
    }
}