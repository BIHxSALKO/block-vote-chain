using System.Collections.Generic;

namespace NikCoin.Networking
{
    public class NodeConnectionInfoComparer : IEqualityComparer<NodeConnectionInfo>
    {
        public bool Equals(NodeConnectionInfo x, NodeConnectionInfo y)
        {
            return x.Ip.Equals(y.Ip) && x.Port.Equals(y.Port);
        }

        public int GetHashCode(NodeConnectionInfo obj)
        {
            return $"{obj.Ip}:{obj.Port}".GetHashCode();
        }
    }
}