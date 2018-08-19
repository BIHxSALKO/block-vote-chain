using System.Collections.Generic;

namespace Pericles.Votes
{
    public static class GenesisVote
    {
        public static readonly List<Vote> Instance =
            new List<Vote> { new Vote("nowhere man", "nowhere plan", "nobody") };
    }
}