using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NikCoin.Hashing;

namespace NikCoin.Transactions
{
    public class Transaction
    {
        public Transaction(List<string> inputs, List<Output> outputs)
        {
            this.VersionNumber = 1;
            this.InCounter = inputs.Count;
            this.OutCounter = outputs.Count;
            this.Inputs = inputs;
            this.Outputs = outputs;
            this.Hash = this.ComputeHash();
        }

        public Transaction(Protocol.Transaction protoTransaction)
        {
            this.VersionNumber = protoTransaction.VersionNumber;
            this.InCounter = protoTransaction.InCounter;
            this.OutCounter = protoTransaction.OutCounter;
            this.Inputs = protoTransaction.Inputs.ToList();
            this.Outputs = protoTransaction.Outputs.Select(x => new Output(x)).ToList();
            this.Hash = this.ComputeHash();
        }

        public int VersionNumber { get; }
        public int InCounter { get; }
        public int OutCounter { get; }
        public List<string> Inputs { get; }
        public List<Output> Outputs { get; }
        public Hash Hash { get; }

        public byte[] GetBytes()
        {
            return BitConverter.GetBytes(this.VersionNumber)
                .Concat(BitConverter.GetBytes(this.InCounter))
                .Concat(BitConverter.GetBytes(this.OutCounter))
                .Concat(this.Inputs.SelectMany(x => Encoding.UTF8.GetBytes(x)))
                .Concat(this.Outputs.SelectMany(x => x.GetBytes()))
                .ToArray();
        }

        public override string ToString()
        {
            return
                $"hash: [{this.Hash}], inputs: [{string.Join(string.Empty, this.Inputs)}], outputs: [{string.Join(string.Empty, this.Outputs)}]";
        }

        private Hash ComputeHash()
        {
            return Sha256DoubleHasher.DoubleHash(this.GetBytes());
        }
    }
}
