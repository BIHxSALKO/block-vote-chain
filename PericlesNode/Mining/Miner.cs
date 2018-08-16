using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using Pericles.Blocks;
using Pericles.Transactions;

namespace Pericles.Mining
{
    public class Miner
    {
        private readonly Blockchain blockchain;
        private readonly TransactionMemoryPool transactionMemoryPool;
        private readonly BigInteger difficultyTarget;
        private readonly CoinbaseTransactionFactory coinbaseTransactionFactory;
        private readonly BlockFactory blockFactory;
        private readonly BlockchainAdder blockchainAdder;
        private readonly object locker;

        private Thread thread;
        private bool shouldAbandonBlock;

        public Miner(
            Blockchain blockchain,
            TransactionMemoryPool transactionMemoryPool,
            BigInteger difficultyTarget,
            CoinbaseTransactionFactory coinbaseTransactionFactory, 
            BlockFactory blockFactory,
            BlockchainAdder blockchainAdder)
        {
            this.blockchain = blockchain;
            this.transactionMemoryPool = transactionMemoryPool;
            this.difficultyTarget = difficultyTarget;
            this.coinbaseTransactionFactory = coinbaseTransactionFactory;
            this.blockFactory = blockFactory;
            this.blockchainAdder = blockchainAdder;
            this.locker = new object();

            this.shouldAbandonBlock = false;
        }

        public void Start()
        {
            this.thread = new Thread(this.MineBlocks) { IsBackground = true };
            this.thread.Start();
        }

        public void AbandonCurrentBlock()
        {
            lock (this.locker)
            {
                this.shouldAbandonBlock = true;
            }
        }

        private void MineBlocks()
        {
            while (true)
            {
                if (this.transactionMemoryPool.Count == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }

                int numTries;
                var nextBlock = this.MineNewBlock(out numTries);
                if (nextBlock == null)
                {
                    continue;
                }

                Console.WriteLine($"\nMINED NEW BLOCK! Took {numTries} tries, hash = {nextBlock.Hash}");
                Console.WriteLine($"{nextBlock}");
                this.blockchainAdder.AddNewBlock(nextBlock);
            }
        }

        private Block MineNewBlock(out int numTries)
        {
            var transactions = this.transactionMemoryPool.GetTransactions(Block.MaxNonCoinbaseTransactions);
            var coinbaseTransaction = this.coinbaseTransactionFactory.Build();
            transactions.Insert(0, coinbaseTransaction);

            var prevBlockHash = this.blockchain.GetLast().Hash;
            var block = this.blockFactory.Build(prevBlockHash, transactions);

            numTries = 1;
            while (true)
            {
                lock (this.locker)
                {
                    if (this.shouldAbandonBlock)
                    {
                        this.shouldAbandonBlock = false;
                        return null;
                    }
                }

                if (!IsValidBlock(block, this.difficultyTarget))
                {
                    block.IncrementNonce();
                    numTries++;
                    continue;
                }

                return block;
            }

            return null;
        }

        private static bool IsValidBlock(Block block, BigInteger target)
        {
            var blockHashAsNumber = block.Hash.ToBigInteger();
            return blockHashAsNumber < target;
        }
    }
}