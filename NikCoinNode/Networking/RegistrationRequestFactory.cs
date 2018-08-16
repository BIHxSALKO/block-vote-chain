using NikCoin.Protocol;
using NikCoin.Utils;

namespace NikCoin.Networking
{
    public class RegistrationRequestFactory
    {
        public RegistrationRequest Build(NodeConnectionInfo myConnectionInfo)
        {
            return new RegistrationRequest
            {
                MyIp = myConnectionInfo.Ip,
                MyPort = myConnectionInfo.Port,
                Time = UnixTimestampGenerator.GetUnixTimestamp(),
                Version = 1
            };
        }
    }
}