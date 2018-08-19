using System;
using System.IO.Pipes;
using Pericles.Blocks;
using Pericles.Crypto;
using Pericles.Merkle;
using Pericles.Mining;
using Pericles.Networking;
using Pericles.Votes;
using VoterDatabase;

namespace Pericles
{
    public static class Program
    {
        private const string Localhost = "localhost";
        private const int RegistrarPort = 50051;
        private const int MinNetworkSize = 3;

        public static void Main(string[] args)
        {
            var password = args[0];
            var dbFilepath = args[1];
            var port = int.Parse(args[2]);

            var voterDb = new VoterDatabaseFacade(dbFilepath);
            var foundMiner = voterDb.TryGetVoterEncryptedKeyPair(password, out var encryptedKeyPair);
            if (!foundMiner)
            {
                Console.WriteLine("incorrect password: you may not mine!");
                return;
            }

            var blockchain = new Blockchain();

            // networking
            var registrarClientFactory = new RegistrarClientFactory();
            var registrarClient = registrarClientFactory.Build(Localhost, RegistrarPort);
            var registrationRequestFactory = new RegistrationRequestFactory();
            var myConnectionInfo = new NodeConnectionInfo(Localhost, port);
            var knownNodeStore = new KnownNodeStore();
            var nodeClientFactory = new NodeClientFactory();
            var handshakeRequestFactory = new HandshakeRequestFactory(blockchain);
            var nodeClientStore = new NodeClientStore();
            var nodeServerFactory = new NodeServerFactory();

            // transactions
            var protoVoteFactory = new ProtoVoteFactory();
            var transactionForwarder = new VoteForwarder(nodeClientStore, protoVoteFactory);
            var transactionMemoryPool = new VoteMemoryPool(transactionForwarder);

            // blocks
            var merkleNodeFactory = new MerkleNodeFactory();
            var merkleTreeFactory = new MerkleTreeFactory(merkleNodeFactory);
            var minerId = encryptedKeyPair.PublicKey.GetBase64String();
            var blockFactory = new BlockFactory(merkleTreeFactory, minerId);
            var protoBlockFactory = new ProtoBlockFactory(protoVoteFactory);
            var blockForwarder = new BlockForwarder(nodeClientStore, protoBlockFactory);
            var blockchainAdder = new BlockchainAdder(blockchain, transactionMemoryPool, blockForwarder);
            var blockValidator = new BlockValidator(blockFactory);
            var voteValidator = new VoteValidator(blockchain);

            // mining
            var difficultyTarget = TargetFactory.Build(BlockHeader.DefaultBits);
            var miner = new Miner(
                blockchain,
                transactionMemoryPool,
                difficultyTarget,
                blockFactory,
                blockchainAdder);

            // startup
            var nodeServer = nodeServerFactory.Build(
                myConnectionInfo,
                knownNodeStore,
                nodeClientFactory,
                nodeClientStore,
                transactionMemoryPool,
                blockchain,
                miner,
                blockValidator,
                blockchainAdder,
                voteValidator);
            var boostrapper = new Bootstrapper(
                MinNetworkSize,
                knownNodeStore,
                nodeClientFactory,
                handshakeRequestFactory,
                nodeClientStore,
                registrarClient,
                registrationRequestFactory,
                nodeServer);

            Console.WriteLine("bootstrapping node network...");
            boostrapper.Bootstrap(myConnectionInfo);
            Console.WriteLine($"{MinNetworkSize} nodes in network! bootstrapping complete");
            

            Console.WriteLine("starting miner...");
            miner.Start();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Press any key to quit");
            Console.ReadKey();
        }
    }
}
