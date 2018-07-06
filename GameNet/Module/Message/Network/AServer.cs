using System;
using System.Net;

namespace GN
{
    public abstract class AServer : Object
    {
        public abstract AClient GetClient(long id);

        private Action<AClient> acceptCallback;
        public event Action<AClient> AcceptCallback
        {
            add
            {
                this.acceptCallback += value;
            }
            remove
            {
                this.acceptCallback -= value;
            }
        }
        protected void OnAccept(AClient client)
        {
            this.acceptCallback.Invoke(client);
        }

        public abstract AClient ConnectClient(IPEndPoint ipEndPoint);
        public abstract void Start();
        public abstract void Remove(long id);
        public abstract void Update();
    }
}
