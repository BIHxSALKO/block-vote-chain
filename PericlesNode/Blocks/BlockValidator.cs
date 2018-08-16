using Pericles.Hashing;

namespace Pericles.Blocks
{
    public class BlockValidator
    {
        private readonly BlockFactory blockFactory;

        public BlockValidator(BlockFactory blockFactory)
        {
            this.blockFactory = blockFactory;
        }

        public bool TryGetValidatedBlock(Protocol.Block protoBlock, out Block reconstructedBlock)
        {
            reconstructedBlock = this.blockFactory.Build(protoBlock);
            var givenHash = new Hash(protoBlock.Hash);
            return reconstructedBlock.Hash.Equals(givenHash);
        }
    }
}