using System.Collections.Generic;

namespace ElectionModels.Interfaces
{
    public interface IElectionAlgorithm
    {
        string GetResults(IEnumerable<string> ballots);
    }
}