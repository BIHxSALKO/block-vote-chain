using System.Linq;
using Google.Protobuf;
using NikCoin.Transactions;

namespace NikCoin.Blocks
{
    public class ProtoBlockFactory
    {
        private readonly ProtoTransactionFactory protoTransactionFactory;

        public ProtoBlockFactory(ProtoTransactionFactory protoTransactionFactory)
        {
            this.protoTransactionFactory = protoTransactionFactory;
        }

        public Protocol.Block Build(Block block)
        {
            var header = block.Header;
            var protoBlockHeader = new Protocol.BlockHeader
            {
                PrevBlockHash = ByteString.CopyFrom(header.PrevBlockHash.GetBytes()),
                MerkleRootHash = ByteString.CopyFrom(header.MerkleRootHash.GetBytes()),
                Timestamp = header.Timestamp,
                Bits = header.Bits,
                Nonce = header.Nonce
            };

            var protoBlock = new Protocol.Block
            {
                BlockHeader = protoBlockHeader,
                Hash = ByteString.CopyFrom(block.Hash.GetBytes()),
                TransactionCounter = block.TransactionCounter,
                Transactions = { }
            };

            var protoTransactions = block.MerkleTree.Transactions.Select(x => this.protoTransactionFactory.Build(x));
            protoBlock.Transactions.AddRange(protoTransactions);

            return protoBlock;
        }
    }
}