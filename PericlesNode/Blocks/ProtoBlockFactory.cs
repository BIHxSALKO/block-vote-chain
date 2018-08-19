using System.Linq;
using Google.Protobuf;
using Pericles.Votes;

namespace Pericles.Blocks
{
    public class ProtoBlockFactory
    {
        private readonly ProtoVoteFactory protoVoteFactory;

        public ProtoBlockFactory(ProtoVoteFactory protoVoteFactory)
        {
            this.protoVoteFactory = protoVoteFactory;
        }

        public Protocol.Block Build(Block block)
        {
            var header = block.Header;
            var protoBlockHeader = new Protocol.BlockHeader
            {
                PrevBlockHash = ByteString.CopyFrom(header.PrevBlockHash.GetBytes()),
                MerkleRootHash = ByteString.CopyFrom(header.MerkleRootHash.GetBytes()),
                Timestamp = header.Timestamp,
                Bits = header.Bits,
                Nonce = header.Nonce
            };

            var protoBlock = new Protocol.Block
            {
                BlockHeader = protoBlockHeader,
                Hash = ByteString.CopyFrom(block.Hash.GetBytes()),
                VoteCounter = block.VoteCounter,
                Votes = { }
            };

            var protoVotes = block.MerkleTree.Votes.Select(x => this.protoVoteFactory.Build(x));
            protoBlock.Votes.AddRange(protoVotes);

            return protoBlock;
        }
    }
}