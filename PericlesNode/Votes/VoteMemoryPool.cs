using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Pericles.Hashing;

namespace Pericles.Votes
{
    public class VoteMemoryPool
    {
        private readonly IOrderedDictionary votePool;
        private readonly VoteForwarder voteForwarder;
        private readonly object locker;

        public VoteMemoryPool(VoteForwarder voteForwarder)
        {
            this.votePool = new OrderedDictionary();
            this.voteForwarder = voteForwarder;
            this.locker = new object();
        }

        public int Count
        {
            get
            {
                lock (this.locker)
                {
                    return this.votePool.Count;
                }
            }
        }

        public bool Contains(Vote vote)
        {
            lock (this.locker)
            {
                return this.votePool.Contains(vote.Hash);
            }
        }

        public void AddVote(Vote vote)
        {
            lock (this.locker)
            {
                if (this.Contains(vote))
                {
                    return;
                }

                Console.WriteLine($"ADDING TXN: {vote.Hash}");
                this.votePool.Add(vote.Hash, vote);
            }

            this.voteForwarder.ForwardVote(vote);
        }

        public List<Vote> GetVotes(int limit)
        {
            lock (this.locker)
            {
                var votes = new List<Vote>();
                var maxToTake = Math.Min(this.votePool.Count, limit);
                for (var i = 0; i < maxToTake; i++)
                {
                    votes.Add((Vote)this.votePool[i]);
                }

                return votes;
            }
        }

        public void DeleteVote(Hash voteHash)
        {
            lock (this.locker)
            {
                this.votePool.Remove(voteHash);
            }
        }
    }
}