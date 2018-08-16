using System.Linq;

namespace NikCoin.Transactions
{
    public class ProtoTransactionFactory
    {
        public Protocol.Transaction Build(Transaction transaction)
        {
            var protoTransaction = new Protocol.Transaction
            {
                VersionNumber = 1,
                InCounter = transaction.InCounter,
                OutCounter = transaction.OutCounter,
                Inputs = { },
                Outputs = { }
            };

            protoTransaction.Inputs.AddRange(transaction.Inputs);
            protoTransaction.Outputs.AddRange(transaction.Outputs.Select(x =>
                new Protocol.Output
                {
                    Value = x.Value,
                    Index = x.Index,
                    Script = x.Script
                }));

            return protoTransaction;
        }
    }
}