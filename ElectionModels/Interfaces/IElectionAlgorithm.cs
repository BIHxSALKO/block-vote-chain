using System.Collections.Generic;

namespace ElectionModels.Interfaces
{
    public interface IElectionAlgorithm
    {
        IWinner GetWinner(IEnumerable<string> ballots);
    }
}