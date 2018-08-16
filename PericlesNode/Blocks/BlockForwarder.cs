using Pericles.Networking;

namespace Pericles.Blocks
{
    public class BlockForwarder
    {
        private readonly NodeClientStore nodeClientStore;
        private readonly ProtoBlockFactory protoBlockFactory;

        public BlockForwarder(
            NodeClientStore nodeClientStore,
            ProtoBlockFactory protoBlockFactory)
        {
            this.nodeClientStore = nodeClientStore;
            this.protoBlockFactory = protoBlockFactory;
        }

        public void ForwardBlock(Block block)
        {
            var protoBlock = this.protoBlockFactory.Build(block);
            foreach (var nodeClient in this.nodeClientStore.GetAll())
            {
                nodeClient.BroadcastBlock(protoBlock);
            }
        }
    }
}