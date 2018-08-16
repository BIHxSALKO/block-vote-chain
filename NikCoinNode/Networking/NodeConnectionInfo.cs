using System;

namespace NikCoin.Networking
{
    public class NodeConnectionInfo : IEquatable<NodeConnectionInfo>
    {
        public NodeConnectionInfo(string ip, int port)
        {
            this.Ip = ip;
            this.Port = port;
        }

        public NodeConnectionInfo(Protocol.ConnectionInfo protoConnectionInfo)
        {
            this.Ip = protoConnectionInfo.IpAddress;
            this.Port = protoConnectionInfo.Port;
        }

        public string Ip { get; }
        public int Port { get; }

        public bool Equals(NodeConnectionInfo other)
        {
            return this.Ip.Equals(other?.Ip) && this.Port.Equals(other?.Port);
        }

        public override string ToString()
        {
            return $"{this.Ip}:{this.Port}";
        }
    }
}