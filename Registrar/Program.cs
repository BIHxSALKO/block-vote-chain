using System;
using Grpc.Core;

namespace Pericles.Registrar
{
    public static class Program
    {
        private const string Localhost = "localhost";
        private const int Port = 50051;

        public static void Main(string[] args)
        {
            var registrarService = new RegistrarService();
            var server = new Server
            {
                Services = { Protocol.Registrar.BindService(registrarService) },
                Ports = { new ServerPort(Localhost, Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Registrar listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
