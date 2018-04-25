using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace HHW.Service
{
    public sealed class TServer : AServer
    {
        private TcpListener acceptor;
        private readonly Dictionary<long, AClient> idClients = new Dictionary<long, AClient>();

        public TServer(IPEndPoint ipEndPoint)
        {
            this.acceptor = new TcpListener(ipEndPoint);
            this.acceptor.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            this.acceptor.Server.NoDelay = true;
            this.acceptor.Start();
        }

        public TServer()
        {

        }

        public override void Dispose()
        {
            if(this.acceptor == null)
            {
                return;
            }
            foreach (long id in this.idClients.Keys.ToArray())
            {
                AClient client = this.idClients[id];
                client.Dispose();
            }
            this.acceptor.Stop();
            this.acceptor = null;

            base.Dispose();
        }

        public override AClient GetClient(long id)
        {
            AClient client = null;
            this.idClients.TryGetValue(id, out client);
            return client;
        }

        public override async Task<AClient> AcceptClient()
        {
            if(this.acceptor == null)
            {
                throw new Exception("server construct must use host and port param");
            }

            TcpClient tcpClient = await this.acceptor.AcceptTcpClientAsync();
            AClient client = new TClient(tcpClient, this);
            this.idClients[client.InstanceId] = client;
            return client;
        }

        public override AClient ConnectClient(IPEndPoint ipEndPoint)
        {
            TcpClient tcpClient = new TcpClient();
            AClient client = new TClient(tcpClient, ipEndPoint, this);
            this.idClients[client.InstanceId] = client;
            return client;
        }

        public override void Remove(long id)
        {
            AClient client;
            if(!this.idClients.Remove(id, out client))
            {
                return;
            }
            if(client == null)
            {
                return;
            }
            client.Dispose();
        }

        public override void Update()
        {
        }
    }
}
