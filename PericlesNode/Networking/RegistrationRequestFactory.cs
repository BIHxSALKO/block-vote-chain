using Pericles.Protocol;
using Pericles.Utils;

namespace Pericles.Networking
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