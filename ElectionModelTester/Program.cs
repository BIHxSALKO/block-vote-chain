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

            var ballot1 = "{ \"candidate\": \"John Doe\" }";
            var ballot2 = "{ \"candidate\": \"Jane Doe\" }";
            var ballot3 = "{ \"candidate\": \"John Doe\" }";
            var ballot4 = "{ \"candidate\": \"Jane Doe\" }";
            var ballot5 = "{ \"candidate\": \"Jane Doe\" }";
            var ballots = new List<string> { ballot1, ballot2, ballot3, ballot4, ballot5 };

            var results = algo.GetResults(ballots);
        }
    }
}
