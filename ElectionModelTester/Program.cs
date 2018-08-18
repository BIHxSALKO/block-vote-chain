using System;
using System.Collections.Generic;
using ElectionModels;
using ElectionModels.FirstPastThePost;

namespace ElectionModelTester
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var voteSerializer = new VoteSerializer();
            var algo = new FirstPastThePostElectionAlgorithm(voteSerializer);

            var ballot1 = new Ballot("{ \"candidate\": \"John Doe\" }");
            var ballot2 = new Ballot("{ \"candidate\": \"Jane Doe\" }");
            var ballot3 = new Ballot("{ \"candidate\": \"John Doe\" }");
            var ballot4 = new Ballot("{ \"candidate\": \"Jane Doe\" }");
            var ballot5 = new Ballot("{ \"candidate\": \"Jane Doe\" }");
            var ballots = new List<IBallot> { ballot1, ballot2, ballot3, ballot4, ballot5 };

            var winner = algo.GetWinner(ballots);
        }
    }
}
