using NikCoin.Protocol;
using NikCoin.Utils;

namespace NikCoin.Networking
{
    public class HandshakeRequestFactory
    {
        private readonly Blockchain blockchain;

        public HandshakeRequestFactory(Blockchain blockchain)
        {
            this.blockchain = blockchain;
        }

        public HandshakeRequest Build(NodeConnectionInfo myConnectionInfo)
        {
            return new HandshakeRequest
            {
                Version = 1,
                Time = UnixTimestampGenerator.GetUnixTimestamp(),
                BestHeight = this.blockchain.CurrentHeight,
                MyConnectionInfo = new ConnectionInfo
                {
                    IpAddress = myConnectionInfo.Ip,
                    Port = myConnectionInfo.Port
                }
            };
        }
    }
}