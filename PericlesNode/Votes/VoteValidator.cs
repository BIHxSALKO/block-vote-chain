using System;
using System.Security.Cryptography;
using Pericles.Crypto;

namespace Pericles.Votes
{
    public class VoteValidator
    {
        private readonly Blockchain blockchain;

        public VoteValidator(Blockchain blockchain)
        {
            this.blockchain = blockchain;
        }

        public bool IsValid(Vote vote)
        {
            return this.IsFirstVoteFromVoter(vote) && IsSignatureValid(vote);
        }

        private bool IsFirstVoteFromVoter(Vote vote)
        {
            var hasVoterAlreadyVoted = this.blockchain.TryGetVoteByVoter(vote.VoterId, out var dummyVote);
            return !hasVoterAlreadyVoted;
        }

        private static bool IsSignatureValid(Vote vote)
        {
            var publicKey = PublicKeyProvider.GetPublicKey(Convert.FromBase64String(vote.VoterId));
            var isSignatureValid = publicKey.VerifyData(
                vote.Ballot.GetBytes(),
                new SHA256CryptoServiceProvider(),
                Convert.FromBase64String(vote.Signature));
            return isSignatureValid;
        }


       
    }
}