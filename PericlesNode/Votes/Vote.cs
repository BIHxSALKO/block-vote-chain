namespace Pericles.Votes
{
    public class Vote
    {
        public Vote(string voterId, string ballot, byte[] signature)
        {
            this.VoterId = voterId;
            this.Ballot = ballot;
            this.Signature = signature;
        }

        public string VoterId { get; }
        public string Ballot { get; }
        public byte[] Signature { get; }
    }
}