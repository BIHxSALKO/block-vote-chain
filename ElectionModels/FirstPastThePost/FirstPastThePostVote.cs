namespace ElectionModels.FirstPastThePost
{
    public class FirstPastThePostVote
    {
        public FirstPastThePostVote(string candidate)
        {
            this.Candidate = candidate;
        }

        public string Candidate { get; }
    }
}