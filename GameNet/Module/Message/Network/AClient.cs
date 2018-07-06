using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace GN
{
    public enum ClientType
    {
        Connect,
        Accept,
    }
    public abstract class AClient : Object
    {
        public ClientType ClientType { get; }

        protected AServer server;

        public IPEndPoint RemoteAddress { get; protected set; }

        private Action<AClient, SocketError> errorCallback;

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

        private Action<Packet> readCallback;
        public event Action<Packet> ReadCallback
        {
            add
            {
                this.readCallback += value;
            }
            remove
            {
                this.readCallback -= value;
            }
        }

        protected void OnRead(Packet packet)
        {
            this.readCallback.Invoke(packet);
        }

        protected AClient(AServer server, ClientType clientType)
        {
            this.ClientType = clientType;
            this.server = server;
        }

        public abstract void Start();

        public abstract void Send(byte[] buffer, int index, int length);

        public abstract void Send(List<byte[]> buffers);

        public override void Dispose()
        {
            if(this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            this.server.Remove(this.id);
        }
    }
}
