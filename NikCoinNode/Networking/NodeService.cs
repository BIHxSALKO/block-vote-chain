using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using NikCoin.Blocks;
using NikCoin.Hashing;
using NikCoin.Mining;
using NikCoin.Protocol;
using NikCoin.Transactions;
using Block = NikCoin.Blocks.Block;

namespace NikCoin.Networking
{
    public class NodeService : Node.NodeBase
    {
        private readonly KnownNodeStore knownNodeStore;
        private readonly NodeClientFactory nodeClientFactory;
        private readonly NodeClientStore nodeClientStore;
        private readonly TransactionMemoryPool transactionMemoryPool;
        private readonly Blockchain blockchain;
        private readonly Miner miner;
        private readonly BlockValidator blockValidator;
        private readonly BlockchainAdder blockchainAdder;

        public NodeService(
            KnownNodeStore knownNodeStore,
            NodeClientFactory nodeClientFactory,
            NodeClientStore nodeClientStore,
            TransactionMemoryPool transactionMemoryPool,
            Blockchain blockchain,
            Miner miner,
            BlockValidator blockValidator,
            BlockchainAdder blockchainAdder)
        {
            this.knownNodeStore = knownNodeStore;
            this.nodeClientFactory = nodeClientFactory;
            this.nodeClientStore = nodeClientStore;
            this.transactionMemoryPool = transactionMemoryPool;
            this.blockchain = blockchain;
            this.miner = miner;
            this.blockValidator = blockValidator;
            this.blockchainAdder = blockchainAdder;
        }

        public override Task<HandshakeResponse> Handshake(HandshakeRequest request, ServerCallContext context)
        {
            var peerConnectionInfo = new NodeConnectionInfo(request.MyConnectionInfo);
            Console.WriteLine($"received handshake from: {peerConnectionInfo}");
            var response = new HandshakeResponse();
            var knownNodes = this.knownNodeStore.GetAll().Select(x => new ConnectionInfo { IpAddress = x.Ip, Port = x.Port });
            response.KnownNodes.AddRange(knownNodes);

            this.knownNodeStore.Add(peerConnectionInfo);
            var nodeClient = this.nodeClientFactory.Build(peerConnectionInfo);
            this.nodeClientStore.Add(nodeClient);

            return Task.FromResult(response);
        }

        public override Task<Empty> BroadcastTransaction(Protocol.Transaction protoTransaction, ServerCallContext context)
        {
            var transaction = new Transactions.Transaction(protoTransaction);
            this.transactionMemoryPool.AddTransaction(transaction);

            // want to fill up blocks with as many transactions as possible, so we
            // ask miner to start over if a new txn comes in and can fit it in block
            if (this.transactionMemoryPool.Count <= Block.MaxNonCoinbaseTransactions)
            {
                this.miner.AbandonCurrentBlock();
            }

            return Task.FromResult(new Empty());
        }

        public override Task<Empty> BroadcastBlock(Protocol.Block protoBlock, ServerCallContext context)
        {
            var hash = new Hash(protoBlock.Hash);
            if (this.blockchain.ContainsBlock(hash))
            {
                return Task.FromResult(new Empty());
            }

            Block block;
            var isValidBlock = this.blockValidator.TryGetValidatedBlock(protoBlock, out block);
            if (!isValidBlock)
            {
                Console.WriteLine("someone tried to trick me! received invalid block.");
                return Task.FromResult(new Empty());
            }

            Console.WriteLine($"RECEIVED BLOCK: {block.Hash}");
            this.blockchainAdder.AddNewBlock(block);
            this.miner.AbandonCurrentBlock();

            return Task.FromResult(new Empty());
        }
    }
}