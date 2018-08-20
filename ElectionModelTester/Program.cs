using System;
using System.Collections.Generic;
using ElectionModels;
using ElectionModels.FirstPastThePost;
using ElectionModels.InstantRunoff;

namespace ElectionModelTester
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var voteSerializer = new VoteSerializer();
            var algo = new InstantRunoffAlgorithm(voteSerializer);
//            var algo = new FirstPastThePostElectionAlgorithm(voteSerializer);
//
//            var ballot1 = "{ \"candidate\": \"John Doe\" }";
//            var ballot2 = "{ \"candidate\": \"Jane Doe\" }";
//            var ballot3 = "{ \"candidate\": \"John Doe\" }";
//            var ballot4 = "{ \"candidate\": \"Jane Doe\" }";
//            var ballot5 = "{ \"candidate\": \"Jane Doe\" }";


            var ballot1 = "{ \"rankOrderedCandidates\": [\"A\", \"B\", \"C\", \"D\", \"E\"] }";
            var ballot2 = "{ \"rankOrderedCandidates\": [\"B\", \"C\", \"D\"] }";
            var ballot3 = "{ \"rankOrderedCandidates\": [\"D\", \"B\", \"C\", \"A\", \"E\"] }";
            var ballot4 = "{ \"rankOrderedCandidates\": [\"E\", \"B\"] }";
            var ballot5 = "{ \"rankOrderedCandidates\": [\"A\", \"B\", \"D\", \"C\", \"E\"] }";
            var ballot6 = "{ \"rankOrderedCandidates\": [\"C\", \"D\", \"A\"] }";
            var ballot7 = "{ \"rankOrderedCandidates\": [\"C\", \"D\", \"E\", \"B\"] }";
            var ballot8 = "{ \"rankOrderedCandidates\": [\"B\", \"E\", \"C\", \"A\"] }";
            var ballot9 = "{ \"rankOrderedCandidates\": [\"E\", \"D\", \"B\"] }";
            var ballots = new List<string> { ballot1, ballot2, ballot3, ballot4, ballot5, ballot6, ballot7, ballot8, ballot9 };

            var results = algo.GetResults(ballots);
            Console.WriteLine(results);
            Console.ReadKey();
        }
    }
}
