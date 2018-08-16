using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Grpc.Core;
using Pericles.Networking;
using Pericles.Protocol;

namespace Pericles
{
    public class Bootstrapper
    {
        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        private readonly int minNetworkSize;
        private readonly KnownNodeStore knownNodeStore;
        private readonly NodeClientFactory nodeClientFactory;
        private readonly HandshakeRequestFactory handshakeRequestFactory;
        private readonly NodeClientStore nodeClientStore;
        private readonly Registrar.RegistrarClient registrarClient;
        private readonly RegistrationRequestFactory registrationRequestFactory;
        private readonly Server nodeServer;


        public Bootstrapper(
            int minNetworkSize, 
            KnownNodeStore knownNodeStore,
            NodeClientFactory nodeClientFactory,
            HandshakeRequestFactory handshakeRequestFactory,
            NodeClientStore nodeClientStore,
            Registrar.RegistrarClient registrarClient,
            RegistrationRequestFactory registrationRequestFactory,
            Server nodeServer)
        {
            this.minNetworkSize = minNetworkSize;
            this.knownNodeStore = knownNodeStore;
            this.nodeClientFactory = nodeClientFactory;
            this.handshakeRequestFactory = handshakeRequestFactory;
            this.nodeClientStore = nodeClientStore;
            this.registrarClient = registrarClient;
            this.registrationRequestFactory = registrationRequestFactory;
            this.nodeServer = nodeServer;
        }

        public void Bootstrap(NodeConnectionInfo myConnectionInfo)
        {
            this.nodeServer.Start();
            var registration = this.registrationRequestFactory.Build(myConnectionInfo);
            var registrationResponse = this.registrarClient.Register(registration);
            var firstNodeConnectionInfo = new NodeConnectionInfo(
                registrationResponse.LastRegisteredNodeIp,
                registrationResponse.LastRegisteredNodePort);

            this.HandshakeWithAllPeers(firstNodeConnectionInfo, myConnectionInfo);
            this.WaitUntilNetworkReachesMinSize();
        }

        private void HandshakeWithAllPeers(
            NodeConnectionInfo firstNodeConnectionInfo, 
            NodeConnectionInfo myConnectionInfo)
        {
            if (string.IsNullOrEmpty(firstNodeConnectionInfo.Ip))
            {
                return;
            }

            var comparer = new NodeConnectionInfoComparer();
            var unmetNodes = new HashSet<NodeConnectionInfo>(comparer) { firstNodeConnectionInfo };
            while (unmetNodes.Any())
            {
                var currentUnmetNode = unmetNodes.First();
                if (currentUnmetNode.Equals(myConnectionInfo))
                {
                    unmetNodes.Remove(currentUnmetNode);
                    continue;
                }

                this.knownNodeStore.Add(currentUnmetNode);
                var nodeClient = this.nodeClientFactory.Build(currentUnmetNode);
                this.nodeClientStore.Add(nodeClient);

                var handshakeRequest = this.handshakeRequestFactory.Build(myConnectionInfo);
                var handshakeResponse = nodeClient.Handshake(handshakeRequest);

                var returnedNodes = handshakeResponse.KnownNodes.Select(x => new NodeConnectionInfo(x));
                var newUnmetNodes = returnedNodes.Where(x => !this.knownNodeStore.Contains(x));
                foreach (var newUnmetNode in newUnmetNodes)
                {
                    unmetNodes.Add(newUnmetNode);
                }

                unmetNodes.Remove(currentUnmetNode);
            }
        }

        private void WaitUntilNetworkReachesMinSize()
        {
            while (this.knownNodeStore.Count < this.minNetworkSize - 1)
            {
                Thread.Sleep(OneSecond);
            }
        }
    }
}