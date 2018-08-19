namespace ElectionModels
{
    public interface IWinner
    {
        string Name { get; }
        string ElectionStats { get; }
        ElectionType ElectionType { get; }
    }

    public class Winner : IWinner
    {
        public Winner(string name, string electionStats, ElectionType electionType)
        {
            this.Name = name;
            this.ElectionStats = electionStats;
            this.ElectionType = electionType;
        }

        public string Name { get; }
        public string ElectionStats { get; }
        public ElectionType ElectionType { get; }
    }
}