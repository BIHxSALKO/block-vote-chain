using NikCoin.Networking;

namespace NikCoin.Transactions
{
    public class TransactionForwarder
    {
        private readonly NodeClientStore nodeClientStore;
        private readonly ProtoTransactionFactory protoTransactionFactory;

        public TransactionForwarder(
            NodeClientStore nodeClientStore,
            ProtoTransactionFactory protoTransactionFactory)
        {
            this.nodeClientStore = nodeClientStore;
            this.protoTransactionFactory = protoTransactionFactory;
        }

        public void ForwardTransaction(Transaction transaction)
        {
            var protoTransaction = this.protoTransactionFactory.Build(transaction);
            foreach (var nodeClient in this.nodeClientStore.GetAll())
            {
                nodeClient.BroadcastTransaction(protoTransaction);
            }
        }
    }
}