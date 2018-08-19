using Grpc.Core;
using Pericles.Blocks;
using Pericles.Mining;
using Pericles.Protocol;
using Pericles.Votes;

namespace Pericles.Networking
{
    public class NodeServerFactory
    {
        public Server Build(
            NodeConnectionInfo myConnectionInfo, 
            KnownNodeStore knownNodeStore,
            NodeClientFactory nodeClientFactory,
            NodeClientStore nodeClientStore,
            VoteMemoryPool transactionMemoryPool,
            Blockchain blockchain,
            Miner miner,
            VoteValidator voteValidator,
            BlockValidator blockValidator,
            BlockchainAdder blockchainAdder)
        {
            var handshakeService = new NodeService(
                knownNodeStore,
                nodeClientFactory,
                nodeClientStore,
                transactionMemoryPool,
                blockchain,
                miner,
                voteValidator,
                blockValidator,
                blockchainAdder);
            var server = new Server
            {
                Services = { Node.BindService(handshakeService) },
                Ports = { new ServerPort(myConnectionInfo.Ip, myConnectionInfo.Port, ServerCredentials.Insecure) }
            };

            return server;
        }
    }
}