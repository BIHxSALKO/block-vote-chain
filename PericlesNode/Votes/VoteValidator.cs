using System;
using System.Security.Cryptography;
using Pericles.Crypto;
using VoterDatabase;

namespace Pericles.Votes
{
    public class VoteValidator
    {
        private readonly VoterDatabaseFacade voterDb;
        private readonly Blockchain blockchain;

        public VoteValidator(Blockchain blockchain, VoterDatabaseFacade voterDb)
        {
            this.blockchain = blockchain;
            this.voterDb = voterDb;
        }

        public bool IsValid(Vote vote)
        {
            return this.IsValidVoter(vote) && this.IsFirstVoteFromVoter(vote) && IsSignatureValid(vote);
        }

        private bool IsValidVoter(Vote vote)
        {
            var isValidVoter = this.voterDb.DoesVoterExist(vote.VoterId);
            return isValidVoter;
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