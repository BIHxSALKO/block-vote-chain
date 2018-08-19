using Pericles.Hashing;
using Pericles.Votes;

namespace Pericles.Blocks
{
    public class BlockValidator
    {
        private readonly BlockFactory blockFactory;
        private readonly VoteValidator voteValidator;

        public BlockValidator(BlockFactory blockFactory, VoteValidator voteValidator)
        {
            this.blockFactory = blockFactory;
            this.voteValidator = voteValidator;
        }

        public bool TryGetValidatedBlock(Protocol.Block protoBlock, out Block reconstructedBlock)
        {
            reconstructedBlock = this.blockFactory.Build(protoBlock);
            var givenHash = new Hash(protoBlock.Hash);
            var hashesMatch = reconstructedBlock.Hash.Equals(givenHash);
            if (!hashesMatch)
            {
                return false;
            }

            foreach (var vote in reconstructedBlock.MerkleTree.Votes)
            {
                if (!this.voteValidator.IsValid(vote))
                {
                    return false;
                }
            }

            return true;
        }
    }
}