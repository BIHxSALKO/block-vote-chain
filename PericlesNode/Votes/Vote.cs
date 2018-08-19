using System.Linq;
using System.Text;
using Pericles.Hashing;

namespace Pericles.Votes
{
    public class Vote
    {
        public Vote(string voterId, string ballot, string signature)
        {
            this.VoterId = voterId;
            this.Ballot = ballot;
            this.Signature = signature;
            this.Hash = this.ComputeHash();
        }

        public Vote(Protocol.Vote protoVote)
        {

        }

        public string VoterId { get; }
        public string Ballot { get; }
        public string Signature { get; }
        public Hash Hash { get; }

        public byte[] GetBytes()
        {
            return Encoding.UTF8.GetBytes(this.VoterId)
                .Concat(Encoding.UTF8.GetBytes(this.Ballot))
                .Concat(Encoding.UTF8.GetBytes(this.Signature))
                .ToArray();
        }

        private Hash ComputeHash()
        {
            return new Sha256DoubleHasher().DoubleHash(this.GetBytes());
        }
    }
}