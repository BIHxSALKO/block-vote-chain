using Pericles.Networking;

namespace Pericles.Votes
{
    public class VoteForwarder
    {
        private readonly NodeClientStore nodeClientStore;
        private readonly ProtoVoteFactory protoVoteFactory;

        public VoteForwarder(
            NodeClientStore nodeClientStore,
            ProtoVoteFactory protoVoteFactory)
        {
            this.nodeClientStore = nodeClientStore;
            this.protoVoteFactory = protoVoteFactory;
        }

        public void ForwardVote(Vote vote)
        {
            var protoVote = this.protoVoteFactory.Build(vote);
            foreach (var nodeClient in this.nodeClientStore.GetAll())
            {
                nodeClient.BroadcastVote(protoVote);
            }
        }
    }
}