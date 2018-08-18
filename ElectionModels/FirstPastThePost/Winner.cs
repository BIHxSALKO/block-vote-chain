namespace ElectionModels.FirstPastThePost
{
    public class Winner : IWinner
    {
        public Winner(string name, string electionStats)
        {
            this.Name = name;
            this.ElectionStats = electionStats;
        }

        public string Name { get; }
        public string ElectionStats { get; }
        public ElectionType ElectionType { get; }
    }
}