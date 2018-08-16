using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using NikCoin.Hashing;

namespace NikCoin.Transactions
{
    public class TransactionMemoryPool
    {
        private readonly IOrderedDictionary transactionPool;
        private readonly TransactionForwarder transactionForwarder;
        private readonly object locker;

        public TransactionMemoryPool(TransactionForwarder transactionForwarder)
        {
            this.transactionPool = new OrderedDictionary();
            this.transactionForwarder = transactionForwarder;
            this.locker = new object();
        }

        public int Count
        {
            get
            {
                lock (this.locker)
                {
                    return this.transactionPool.Count;
                }
            }
        }

        public bool Contains(Transaction transaction)
        {
            lock (this.locker)
            {
                return this.transactionPool.Contains(transaction.Hash);
            }
        }

        public void AddTransaction(Transaction transaction)
        {
            lock (this.locker)
            {
                if (this.Contains(transaction))
                {
                    return;
                }

                Console.WriteLine($"ADDING TXN: {transaction.Hash}");
                this.transactionPool.Add(transaction.Hash, transaction);
            }

            this.transactionForwarder.ForwardTransaction(transaction);
        }

        public List<Transaction> GetTransactions(int limit)
        {
            lock (this.locker)
            {
                var transactions = new List<Transaction>();
                var maxToTake = Math.Min(this.transactionPool.Count, limit);
                for (var i = 0; i < maxToTake; i++)
                {
                    transactions.Add((Transaction)this.transactionPool[i]);
                }

                return transactions;
            }
        }

        public void DeleteTransaction(Hash transactionHash)
        {
            lock (this.locker)
            {
                this.transactionPool.Remove(transactionHash);
            }
        }
    }
}