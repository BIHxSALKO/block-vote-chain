namespace ElectionModels
{
    public interface IWinner
    {
        string Name { get; }
        string ElectionStats { get; }
        ElectionType ElectionType { get; }
    }
}