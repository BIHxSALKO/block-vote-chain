using System.Collections.Generic;
using System.Collections.Specialized;
using Pericles.Blocks;
using Pericles.Hashing;
using Pericles.Transactions;

namespace Pericles
{
    public class Blockchain
    {
        private readonly OrderedDictionary blockStore; // allows lookup by key and by index
        private readonly Dictionary<Hash, Transaction> transactionDict;
        private readonly object locker;

        public Blockchain()
        {
            this.blockStore = new OrderedDictionary();
            this.transactionDict = new Dictionary<Hash, Transaction>();
            this.locker = new object();

            var genesisBlock = GenesisBlock.Instance;
            this.AddBlock(genesisBlock);
        }

        public int CurrentHeight
        {
            get
            {
                lock (this.locker)
                {
                    return this.blockStore.Count - 1;
                }
            }
        }

        public bool ContainsBlock(Hash hash)
        {
            lock (this.locker)
            {
                return this.blockStore.Contains(hash);
            }
        }

        public void AddBlock(Block block)
        {
            lock (this.locker)
            {
                if (this.ContainsBlock(block.Hash))
                {
                    return;
                }

                this.blockStore.Add(block.Hash, block);
                foreach (var transaction in block.MerkleTree.Transactions)
                {
                    this.transactionDict.Add(transaction.Hash, transaction);
                }
            }
        }

        public Block GetLast()
        {
            lock (this.locker)
            {
                Block block;
                this.TryGetBlockByHeight(this.CurrentHeight, out block);
                return block;
            }
        }

        public bool TryGetBlockByHeight(int height, out Block block)
        {
            lock (this.locker)
            {
                if (height > this.CurrentHeight)
                {
                    block = null;
                    return false;
                }

                block = this.blockStore[height] as Block;
                return block != null;
            }
        }

        public bool TryGetBlockByHash(Hash blockHash, out Block block)
        {
            lock (this.locker)
            {
                if (!this.blockStore.Contains(blockHash))
                {
                    block = null;
                    return false;
                }

                block = this.blockStore[blockHash] as Block;
                return block != null;
            }
        }

        public bool TryGetTransactionByHash(Hash transactionHash, out Transaction transaction)
        {
            lock (this.locker)
            {
                return this.transactionDict.TryGetValue(transactionHash, out transaction);
            }
        }
    }
}
