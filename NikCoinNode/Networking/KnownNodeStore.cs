using System.Collections.Generic;
using System.Linq;

namespace NikCoin.Networking
{
    public class KnownNodeStore
    {
        private readonly HashSet<NodeConnectionInfo> knownNodes;
        private readonly object locker;

        public KnownNodeStore()
        {
            var comparer = new NodeConnectionInfoComparer();
            this.knownNodes = new HashSet<NodeConnectionInfo>(comparer);
            this.locker = new object();
        }

        public int Count
        {
            get
            {
                lock (this.locker)
                {
                    return this.knownNodes.Count;
                }
            }
        }

        public bool Contains(NodeConnectionInfo node)
        {
            lock (this.locker)
            {
                return this.knownNodes.Contains(node);
            }
        }

        public void Add(NodeConnectionInfo node)
        {
            lock (this.locker)
            {
                this.knownNodes.Add(node);
            }
        }

        public IReadOnlyList<NodeConnectionInfo> GetAll()
        {
            lock (this.locker)
            {
                return this.knownNodes.ToList();
            }
        }
    }
}