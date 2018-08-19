using System;
using System.Collections.Generic;
using System.Linq;
using Pericles.Votes;

namespace Pericles.Blocks
{
    public class BlockchainAdder
    {
        private readonly Blockchain blockchain;
        private readonly VoteMemoryPool transactionMemoryPool;
        private readonly BlockForwarder blockForwarder;

        public BlockchainAdder(
            Blockchain blockchain,
            VoteMemoryPool transactionMemoryPool,
            BlockForwarder blockForwarder)
        {
            this.blockchain = blockchain;
            this.transactionMemoryPool = transactionMemoryPool;
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

        private void RemoveVotesFromMemPool(IEnumerable<Vote> transactions)
        {
            foreach (var vote in transactions.Skip(1))
            {
                this.transactionMemoryPool.DeleteVote(vote.Hash);
            }
        }
    }
}