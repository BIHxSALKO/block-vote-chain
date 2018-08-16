using System.Collections.Generic;
using System.Linq;
using NikCoin.Protocol;

namespace NikCoin.Networking
{
    public class NodeClientStore
    {
        private readonly List<Node.NodeClient> nodeClients;

        public NodeClientStore()
        {
            this.nodeClients = new List<Node.NodeClient>();
        }

        public void Add(Node.NodeClient nodeClient)
        {
            this.nodeClients.Add(nodeClient);
        }

        public IReadOnlyList<Node.NodeClient> GetAll()
        {
            return this.nodeClients.ToList();
        }
    }
}