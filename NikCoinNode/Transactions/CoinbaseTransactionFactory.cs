using System;
using System.Collections.Generic;

namespace NikCoin.Transactions
{
    public class CoinbaseTransactionFactory
    {
        private const int BlockReward = 750 * 1000; // 750 "NikCoins", which are denominated in thousandths

        private readonly Random random;

        public CoinbaseTransactionFactory()
        {
            this.random = new Random();
        }

        public Transaction Build()
        {
            var output = new Output(BlockReward, 0, $"coinbase txn at {DateTime.Now:HH:mm:ss.ffffff} + random double {this.random.NextDouble()}");
            var outputs = new List<Output> { output };
            var inputs = new List<string> { "coinbase" };
            var coinbaseTransaction = new Transaction(inputs, outputs);
            return coinbaseTransaction;
        }
    }
}