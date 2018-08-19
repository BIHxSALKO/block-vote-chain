namespace Pericles.Votes
{
    public class ProtoVoteFactory
    {
        public Protocol.Vote Build(Vote vote)
        {
            var protoVote = new Protocol.Vote
            {
                VersionNumber = 1,
                VoterId = vote.VoterId,
                Ballot = vote.Ballot,
                Signature = vote.Signature
            };

            return protoVote;
        }
    }
}