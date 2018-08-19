using Newtonsoft.Json;

namespace ElectionModels
{
    public interface IVoteSerializer
    {
        string Serialize<T>(T vote);
        T Deserialize<T>(string voteJson);
    }

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