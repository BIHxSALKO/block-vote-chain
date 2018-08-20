using System;
using System.Collections.Generic;
using System.Linq;
using ElectionModels.Interfaces;

namespace ElectionModels.InstantRunoff
{
    public class InstantRunoffAlgorithm : IElectionAlgorithm
    {
        private readonly IVoteSerializer voteSerializer;

        public InstantRunoffAlgorithm(IVoteSerializer voteSerializer)
        {
            this.voteSerializer = voteSerializer;
        }

        public string GetResults(IEnumerable<string> ballots)
        {

            var allRankedVotes = ballots.Select(x => this.voteSerializer.Deserialize<InstantRunoffVote>(x)).ToList();
            var firstPrefVotes = this.InitializeFirstPrefVotes(allRankedVotes);
            var winningThreshold = allRankedVotes.Count / 2.0;
            var candidatesEliminated = new List<string>();
            var winner = "";
            var restString = "";

            while (firstPrefVotes.Keys.Count >= 2)
            {
                foreach (var candidate in firstPrefVotes.Keys)
                {
                    if (firstPrefVotes[candidate] > winningThreshold)
                    {
                        winner = candidate;
                        var restRanked = firstPrefVotes.OrderByDescending(x => x.Value).ToList();
                        foreach (var losingCandidate in restRanked)
                        {
                            if (restString == "")
                            {
                                restString += losingCandidate.Key;
                            }
                            else
                            {
                                restString += ", " + losingCandidate.Key;
                            }
                        }
                        break;
                    }
                }

                if (winner != "")
                {
                    break;
                }
                else
                {
                    var candidateToEliminate = CandidateToEliminate(firstPrefVotes);
                    firstPrefVotes = RedistributeVotes(candidateToEliminate, candidatesEliminated, firstPrefVotes,
                        allRankedVotes);
                    candidatesEliminated.Add(candidateToEliminate);
                }
            }

            for (int i = candidatesEliminated.Count - 1; i > -1; i--)
            {
                if (i == 0)
                {
                    restString += (", " + candidatesEliminated[i] + ".");
                }
                else if (restString == "")
                {
                    restString += candidatesEliminated[i];
                }
                else
                {
                    restString += (", " + candidatesEliminated[i]);
                }
            }

            return ("The winner is: " + winner + ".\n The rest of the candidates finished in this order: " +
                    restString);

            throw new Exception("Couldn't find a winner! Your election is compromised, comrade!");
        }

        private Dictionary<string, int> InitializeFirstPrefVotes(IEnumerable<InstantRunoffVote> allRankedVotes)
        {
            var rankDict = new Dictionary<string, int>();
            // tallies 1st pref votes 
            foreach (var rankedVote in allRankedVotes)
            {
                if (rankDict.ContainsKey(rankedVote.RankOrderedCandidates[0]))
                {
                    rankDict[rankedVote.RankOrderedCandidates[0]] += 1;
                }
                else
                {
                    rankDict[rankedVote.RankOrderedCandidates[0]] = 1;
                }
            }

            return rankDict;
        }

        private string CandidateToEliminate(Dictionary<string, int> firstPrefVotes)
        {
            var candidateToEliminate = "Donald Trump";
            var minVotes = int.MaxValue;
            // figures out which candidate to eliminate 
            foreach (var candidate in firstPrefVotes.Keys)
            {
                if (firstPrefVotes[candidate] < minVotes)
                {
                    candidateToEliminate = candidate;
                    minVotes = firstPrefVotes[candidate];
                }
            }

            return candidateToEliminate;
        }

        private Dictionary<string, int> RedistributeVotes(string eliminatedCandidate, List<string> candidatesEliminated,
            Dictionary<string, int> firstPrefVotes, IEnumerable<InstantRunoffVote> allRankedVotes)
        {
            foreach (var rankedVote in allRankedVotes)
            {
                foreach (var candidate in rankedVote.RankOrderedCandidates)
                {
                    // if candidate has not been eliminated
                    if ((candidatesEliminated.Contains(candidate) == false))
                    {
                        // if this non-eliminated candidate is not who is currently being eliminated, ignore this ballot, 
                        // as there are no redistributions to be done
                        if (candidate != eliminatedCandidate)
                        {
                            break;
                        }
                        // otherwise, this is a ballot where the first non-eliminated candidate is the same candidate that is now
                        // going to be eliminated, so we have to redistribute this ballot's votes for non-eliminated candidates
                        else
                        {
                            // unless the ballot contains no further votes
                            if (rankedVote.RankOrderedCandidates.IndexOf(candidate) ==
                                rankedVote.RankOrderedCandidates.Count - 1)
                            {
                                break;
                            }

                            // otherwise look at the remaining votes on the ballot (after the candidate who is being eliminated), find the first
                            // which is for a candidate who has not been eliminated, and give this candidate a new first pref vote
                            for (int i = rankedVote.RankOrderedCandidates.IndexOf(candidate) + 1;
                                i < rankedVote.RankOrderedCandidates.Count;
                                i++)
                            {
                                if (candidatesEliminated.Contains(rankedVote.RankOrderedCandidates[i]) == false)
                                {
                                    firstPrefVotes[rankedVote.RankOrderedCandidates[i]] += 1;
                                    break;
                                }
                            }

                            break;
                        }
                    }
                }
            }

            firstPrefVotes.Remove(eliminatedCandidate);
            return firstPrefVotes;
        }

    }

}