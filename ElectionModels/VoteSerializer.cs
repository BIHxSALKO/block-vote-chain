using Newtonsoft.Json;

namespace ElectionModels
{
    public class VoteSerializer : IVoteSerializer
    {
        public string Serialize<T>(T vote)
        {
            return JsonConvert.SerializeObject(vote);
        }

        public T Deserialize<T>(string voteJson)
        {
            return JsonConvert.DeserializeObject<T>(voteJson);
        }
    }
}