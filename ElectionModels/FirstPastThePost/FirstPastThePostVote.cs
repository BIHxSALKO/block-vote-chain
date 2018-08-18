using Newtonsoft.Json;

namespace ElectionModels.FirstPastThePost
{
    public class FirstPastThePostVote
    {
        public FirstPastThePostVote(string candidate)
        {
            this.Candidate = candidate;
        }

        [JsonProperty(PropertyName = "candidate")]
        public string Candidate { get; }
    }
}