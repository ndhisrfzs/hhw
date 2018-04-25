using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace HHW.Service
{
    public enum ClientType
    {
        Connect,
        Accept,
    }
    public abstract class AClient : Component
    {
        public ClientType ClientType { get; }

        protected AServer server;

        public IPEndPoint RemoteAddress { get; protected set; }

        private event Action<AClient, SocketError> errorCallback;

        public event Action<AClient, SocketError> ErrorCallback
        {
            add
            {
                this.errorCallback += value;
            }
            remove
            {
                this.errorCallback -= value;
            }
        }

        protected void OnError(SocketError e)
        {
            if(this.IsDisposed)
            {
                return;
            }
            this.errorCallback?.Invoke(this, e);
        }

        protected AClient(AServer server, ClientType clientType)
        {
            this.ClientType = clientType;
            this.server = server;
        }

        public abstract void Send(byte[] buffer, int index, int length);

        public abstract void Send(List<byte[]> buffers);

        public abstract Task<Packet> Recv();

        public override void Dispose()
        {
            if(this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            this.server.Remove(this.InstanceId);
        }
    }
}
