using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Pericles.Blocks;
using Pericles.Hashing;
using Pericles.Mining;
using Pericles.Protocol;
using Pericles.Votes;
using Block = Pericles.Blocks.Block;

namespace Pericles.Networking
{
    public class NodeService : Node.NodeBase
    {
        private readonly KnownNodeStore knownNodeStore;
        private readonly NodeClientFactory nodeClientFactory;
        private readonly NodeClientStore nodeClientStore;
        private readonly VoteMemoryPool voteMemoryPool;
        private readonly Blockchain blockchain;
        private readonly Miner miner;
        private readonly VoteValidator voteValidator;
        private readonly BlockValidator blockValidator;
        private readonly BlockchainAdder blockchainAdder;

        public NodeService(
            KnownNodeStore knownNodeStore,
            NodeClientFactory nodeClientFactory,
            NodeClientStore nodeClientStore,
            VoteMemoryPool voteMemoryPool,
            Blockchain blockchain,
            Miner miner,
            VoteValidator voteValidator,
            BlockValidator blockValidator,
            BlockchainAdder blockchainAdder)
        {
            this.knownNodeStore = knownNodeStore;
            this.nodeClientFactory = nodeClientFactory;
            this.nodeClientStore = nodeClientStore;
            this.voteMemoryPool = voteMemoryPool;
            this.blockchain = blockchain;
            this.miner = miner;
            this.voteValidator = voteValidator;
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

        public override Task<Empty> BroadcastVote(Protocol.Vote protoVote, ServerCallContext context)
        {
            var vote = new Votes.Vote(protoVote);
            if (!this.voteValidator.IsValid(vote))
            {
                return Task.FromResult(new Empty());
            }
            
            this.voteMemoryPool.AddVote(vote);

            // want to fill up blocks with as many votes as possible, so we
            // ask miner to start over if a new txn comes in and can fit it in block
            if (this.voteMemoryPool.Count <= Block.MaxVotes)
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