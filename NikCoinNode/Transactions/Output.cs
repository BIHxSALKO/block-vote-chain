using System;
using System.Linq;
using System.Text;

namespace NikCoin.Transactions
{
    public class Output
    {
        public Output(int value, int index, string script)
        {
            this.Value = value;
            this.Index = index;
            this.Script = script;
        }

        public Output(Protocol.Output protoOutput)
        {
            this.Value = protoOutput.Value;
            this.Index = protoOutput.Index;
            this.Script = protoOutput.Script;
        }

        public int Value { get; }
        public int Index { get; }
        public string Script { get; }

        public byte[] GetBytes()
        {
            return BitConverter.GetBytes(this.Value)
                .Concat(BitConverter.GetBytes(this.Index))
                .Concat(Encoding.UTF8.GetBytes(this.Script))
                .ToArray();
        }

        public override string ToString()
        {
            return $"{this.Script}, Value = {this.Value}";
        }
    }
}