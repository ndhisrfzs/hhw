using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace GN
{
    public sealed class TServer : AServer
    {
        private Socket listener;
        private readonly SocketAsyncEventArgs innArgs = new SocketAsyncEventArgs();
        private readonly Dictionary<long, AClient> idClients = new Dictionary<long, AClient>();

        public TServer(IPEndPoint ipEndPoint)
        {
            this.listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            this.listener.NoDelay = true;
            this.innArgs.Completed += this.OnAcceptComplete;

            this.listener.Bind(ipEndPoint);
            this.listener.Listen(1000);
        }

        public TServer()
        {

        }

        public override void Dispose()
        {
            if (this.listener == null)
            {
                return;
            }

            base.Dispose();

            foreach (long id in this.idClients.Keys.ToArray())
            {
                AClient client = this.idClients[id];
                client.Dispose();
            }
            this.listener?.Close();
            this.innArgs.Dispose();
            this.listener = null;
        }

        public override void Start()
        {
            if(this.listener != null)
            {
                this.AcceptAsync();
            }
        }

        private void OnAcceptComplete(object sender, SocketAsyncEventArgs o)
        {
            if(this.listener == null)
            {
                return;
            }

            SocketAsyncEventArgs e = o;
            if(e.SocketError != SocketError.Success)
            {
                return;
            }

            TClient client = new TClient(e.AcceptSocket, this);
            this.idClients[client.id] = client;
            try
            {
                this.OnAccept(client);
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }

            if(this.listener == null)
            {
                return;
            }

            this.AcceptAsync();
        }

        private void AcceptAsync()
        {
            this.innArgs.AcceptSocket = null;
            if (this.listener.AcceptAsync(this.innArgs))
            {
                return;
            }
            OnAcceptComplete(this, this.innArgs);
        }

        public override AClient GetClient(long id)
        {
            AClient client = null;
            this.idClients.TryGetValue(id, out client);
            return client;
        }

        public override AClient ConnectClient(IPEndPoint ipEndPoint)
        {
            TClient client = new TClient(ipEndPoint, this);
            this.idClients[client.id] = client;
            return client;
        }

        public override void Remove(long id)
        {
            AClient client;
            if(!this.idClients.TryGetValue(id, out client))
            {
                return;
            }
            if(client == null)
            {
                return;
            }
            this.idClients.Remove(id);
            client.Dispose();
        }

        public override void Update()
        {
        }
    }
}
