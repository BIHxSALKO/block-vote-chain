using Grpc.Core;
using Pericles.Protocol;

namespace Pericles.Networking
{
    public class RegistrarClientFactory
    {
        public Registrar.RegistrarClient Build(string ip, int port)
        {
            var ipAndPortString = $"{ip}:{port}";
            var channel = new Channel(ipAndPortString, ChannelCredentials.Insecure);
            return new Registrar.RegistrarClient(channel);
        }
    }
}