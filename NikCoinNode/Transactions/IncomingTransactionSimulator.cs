using System;
using System.Collections.Generic;
using System.Threading;

namespace NikCoin.Transactions
{
    public class IncomingTransactionSimulator
    {
        private const int MinSleepMillis = 5000;
        private const int MaxSleepMillis = 10000;

        private readonly TransactionMemoryPool transactionMemoryPool;
        private readonly Random random;

        private Thread thread;

        public IncomingTransactionSimulator(TransactionMemoryPool transactionMemoryPool)
        {
            this.transactionMemoryPool = transactionMemoryPool;
            this.random = new Random();
        }

        public void GenerateNewUniqueTransaction()
        {
            var inputs = new List<string> { $"{DateTime.Now:HH:mm:ss.ffffff}" };
            var outputs = new List<Output> { new Output(0, 0, $"{DateTime.UtcNow:HH:mm:ss.ffffff}") };
            var transaction = new Transaction(inputs, outputs);

            Console.WriteLine($"GENERATED TXN: {transaction.Hash}");
            this.transactionMemoryPool.AddTransaction(transaction);
        }

        public void Start()
        {
            this.thread = new Thread(this.AddNewTransactionsToPool) { IsBackground = true };
            this.thread.Start();
        }

        private void AddNewTransactionsToPool()
        {
            while (true)
            {
                this.GenerateNewUniqueTransaction();
                var randomSleep = this.random.Next(MinSleepMillis, MaxSleepMillis);
                Thread.Sleep(randomSleep);
            }
        }
    }
}