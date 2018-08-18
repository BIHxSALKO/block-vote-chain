using System.Collections.Generic;

namespace ElectionModels
{
    public interface IElectionAlgorithm
    {
        IWinner GetWinner(List<IBallot> ballots);
    }
}