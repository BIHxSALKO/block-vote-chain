namespace ElectionModels
{
    public class Ballot : IBallot
    {
        public Ballot(string contents)
        {
            this.Contents = contents;
        }

        public string Contents { get; }
    }
}