namespace ElectionModels
{
    public interface IVoteSerializer
    {
        string Serialize<T>(T vote);
        T Deserialize<T>(string voteJson);
    }
}