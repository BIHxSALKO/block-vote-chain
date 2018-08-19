using System.Collections.Generic;
using System.Collections.Specialized;
using Pericles.Blocks;
using Pericles.Hashing;
using Pericles.Votes;

namespace Pericles
{
    public class Blockchain
    {
        private readonly OrderedDictionary blockStore; // allows lookup by key and by index
        private readonly Dictionary<Hash, Vote> votesDictByHash;
        private readonly Dictionary<string, Vote> votesDictByVoter;
        private readonly object locker;

        public Blockchain()
        {
            this.blockStore = new OrderedDictionary();
            this.votesDictByHash = new Dictionary<Hash, Vote>();
            this.votesDictByVoter = new Dictionary<string, Vote>();
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

        public bool ContainsVoter(string voterId)
        {
            lock (this.locker)
            {
                return this.votesDictByVoter.ContainsKey(voterId);
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
                foreach (var vote in block.MerkleTree.Votes)
                {
                    this.votesDictByHash.Add(vote.Hash, vote);
                    this.votesDictByVoter.Add(vote.VoterId, vote);
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

        public bool TryGetVoteByHash(Hash voteHash, out Vote vote)
        {
            lock (this.locker)
            {
                return this.votesDictByHash.TryGetValue(voteHash, out vote);
            }
        }

        public bool TryGetVoteByVoter(string voterId, out Vote vote)
        {
            lock (this.locker)
            {
                return this.votesDictByVoter.TryGetValue(voterId, out vote);
            }
        }
    }
}
