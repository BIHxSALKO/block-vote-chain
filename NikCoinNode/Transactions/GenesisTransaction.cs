using System.Collections.Generic;

namespace NikCoin.Transactions
{
    public static class GenesisTransaction
    {
        private static readonly List<string> GenesisInputs = new List<string> { "Jabberwocky" };
        private static readonly List<Output> GenesisOutputs = new List<Output> { new Output(0, 0, "By Lewis Carroll") };

        public static readonly List<Transaction> Instance =
            new List<Transaction> {new Transaction(GenesisInputs, GenesisOutputs)};
    }
}
