using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ElectionModels.InstantRunoff
{
    public class InstantRunoffVote
    {
        public InstantRunoffVote(List<string> rankOrderedCandidates)
        {
            this.RankOrderedCandidates = rankOrderedCandidates;
        }

        [JsonProperty(PropertyName = "rankOrderedCandidates")]
        public List<string> RankOrderedCandidates { get; }
    }
}