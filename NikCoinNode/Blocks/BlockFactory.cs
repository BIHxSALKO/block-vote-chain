using System.Collections.Generic;
using System.Linq;
using NikCoin.Hashing;
using NikCoin.Merkle;
using NikCoin.Transactions;

namespace NikCoin.Blocks
{
    public class BlockFactory
    {
        private readonly MerkleTreeFactory merkleTreeFactory;

        public BlockFactory(MerkleTreeFactory merkleTreeFactory)
        {
            this.merkleTreeFactory = merkleTreeFactory;
        }

        public Block Build(Hash prevBlockHash, List<Transaction> transactions)
        {
            var merkleTree = this.merkleTreeFactory.BuildMerkleTree(transactions);
            var blockHeader = new BlockHeader(prevBlockHash, merkleTree.Root.Hash);
            return new Block(blockHeader, merkleTree);
        }

        public Block Build(Protocol.Block protoBlock)
        {
            var transactions = protoBlock.Transactions.Select(x => new Transaction(x)).ToList();
            var merkleTree = this.merkleTreeFactory.BuildMerkleTree(transactions);
            var blockHeader = new BlockHeader(protoBlock.BlockHeader);
            return new Block(blockHeader, merkleTree);
        }
    }
}