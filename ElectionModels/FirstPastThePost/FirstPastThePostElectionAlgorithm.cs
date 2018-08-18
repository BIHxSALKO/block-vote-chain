using System;
using System.Collections.Generic;
using System.Linq;

namespace ElectionModels.FirstPastThePost
{
    public class FirstPastThePostElectionAlgorithm : IElectionAlgorithm
    {
        public IWinner GetWinner(List<IBallot> ballots)
        {
            var voteCountDict = new Dictionary<string, int>();
            foreach (var ballot in ballots)
            {
                var contents = ballot.Contents;
                // parse JSON into FirstPastThePostVote class
                var vote = new FirstPastThePostVote(string.Empty);

                var foundCandidate = voteCountDict.TryGetValue(vote.Candidate, out var voteCount);
                if (!foundCandidate)
                {
                    voteCountDict[vote.Candidate] = 1;
                }
                else
                {
                    voteCountDict[vote.Candidate] = voteCount + 1;
                }
            }

            var mostVotes = voteCountDict.Values.Max();
            var winners = voteCountDict.Where(x => x.Value == mostVotes).ToList();
            if (winners.Count > 1)
            {
                throw new Exception("it's a tie! oh no");
            }

            var winner = new Winner(winners[0].Key, $"# of votes: {winners[0].Value}", ElectionType.FirstPastThePost);
            return winner;
        }
    }
}