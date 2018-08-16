using Grpc.Core;
using Pericles.Protocol;

namespace Pericles.Networking
{
    public class NodeClientFactory
    {
        public Node.NodeClient Build(NodeConnectionInfo connectionInfo)
        {
            var ipAndPortString = $"{connectionInfo.Ip}:{connectionInfo.Port}";
            var channel = new Channel(ipAndPortString, ChannelCredentials.Insecure);
            return new Node.NodeClient(channel);
        }
    }
}