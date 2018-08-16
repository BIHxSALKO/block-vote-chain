using System;
using System.Threading.Tasks;
using Grpc.Core;
using Pericles.Protocol;

namespace Pericles.Registrar
{
    public class RegistrarService : Protocol.Registrar.RegistrarBase
    {
        private readonly object locker;

        private string lastRegisteredNodeIp;
        private int lastRegisteredNodePort;

        public RegistrarService()
        {
            this.locker = new object();
            this.lastRegisteredNodeIp = string.Empty;
            this.lastRegisteredNodePort = -1;
        }

        public override Task<RegistrationResponse> Register(RegistrationRequest request, ServerCallContext context)
        {
            lock (this.locker)
            {
                Console.WriteLine($"registration received from: {request.MyIp}:{request.MyPort}");

                var response = new RegistrationResponse
                {
                    LastRegisteredNodeIp = this.lastRegisteredNodeIp,
                    LastRegisteredNodePort = this.lastRegisteredNodePort
                };

                this.lastRegisteredNodeIp = request.MyIp;
                this.lastRegisteredNodePort = request.MyPort;
                return Task.FromResult(response);
            }
        }
    }
}