using System;
using NikCoin.Blocks;
using NikCoin.Merkle;
using NikCoin.Mining;
using NikCoin.Networking;
using NikCoin.Transactions;

namespace NikCoin
{
    public static class Program
    {
        private const string Localhost = "localhost";
        private const int RegistrarPort = 50051;
        private const int MinNetworkSize = 3;

        public static void Main(string[] args)
        {
            var blockchain = new Blockchain();

            // networking
            var registrarClientFactory = new RegistrarClientFactory();
            var registrarClient = registrarClientFactory.Build(Localhost, RegistrarPort);
            var registrationRequestFactory = new RegistrationRequestFactory();
            var nodePort = int.Parse(args[0]);
            var myConnectionInfo = new NodeConnectionInfo(Localhost, nodePort);
            var knownNodeStore = new KnownNodeStore();
            var nodeClientFactory = new NodeClientFactory();
            var handshakeRequestFactory = new HandshakeRequestFactory(blockchain);
            var nodeClientStore = new NodeClientStore();
            var nodeServerFactory = new NodeServerFactory();

            // transactions
            var protoTransactionFactory = new ProtoTransactionFactory();
            var transactionForwarder = new TransactionForwarder(nodeClientStore, protoTransactionFactory);
            var transactionMemoryPool = new TransactionMemoryPool(transactionForwarder);
            var incomingTransactionSimulator = new IncomingTransactionSimulator(transactionMemoryPool);

            // blocks
            var merkleNodeFactory = new MerkleNodeFactory();
            var merkleTreeFactory = new MerkleTreeFactory(merkleNodeFactory);
            var blockFactory = new BlockFactory(merkleTreeFactory);
            var protoBlockFactory = new ProtoBlockFactory(protoTransactionFactory);
            var blockForwarder = new BlockForwarder(nodeClientStore, protoBlockFactory);
            var blockchainAdder = new BlockchainAdder(blockchain, transactionMemoryPool, blockForwarder);
            var blockValidator = new BlockValidator(blockFactory);

            // mining
            var difficultyTarget = TargetFactory.Build(BlockHeader.DefaultBits);
            var coinbaseTransactionFactory = new CoinbaseTransactionFactory();
            var miner = new Miner(
                blockchain,
                transactionMemoryPool,
                difficultyTarget,
                coinbaseTransactionFactory, 
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
                blockchainAdder);
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
            
            Console.WriteLine("simulating incoming transactions...");
            incomingTransactionSimulator.Start();

            Console.WriteLine("starting miner...");
            miner.Start();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Press any key to quit");
            Console.ReadKey();
        }
    }
}
