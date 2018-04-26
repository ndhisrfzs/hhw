using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HHW.Service
{
    public abstract class NetworkComponent : Component
    {
        private AServer Server;

        private readonly Dictionary<long, Session> sessions = new Dictionary<long, Session>();

        public IMessagePacker MessagePacker { get; set; }
        public IMessageDispatcher MessageDispatcher { get; set; }

        public void Awake(NetworkComponent self)
        {
            this.Server = new TServer();
        }

        public void Awake(IPEndPoint ipEndPoint)
        {
            this.Server = new TServer(ipEndPoint);

            this.StartAccept();
        }

        private async void StartAccept()
        {
            while(true)
            {
                if(this.IsDisposed)
                {
                    return;
                }

                await this.Accept();
            }
        }

        public virtual async Task<Session> Accept()
        {
            AClient client = await this.Server.AcceptClient();
            Session session = ObjectFactory.Create<Session, NetworkComponent, AClient>(this, client);
            session.Network = this;
            client.ErrorCallback += (c, e) => { this.Remove(session.id); };
            this.sessions.Add(session.id, session);
            return session;
        }

        public virtual void Remove(long id)
        {
            Session session;
            if(!this.sessions.TryGetValue(id, out session))
            {
                return;
            }
            this.sessions.Remove(id);
            session.Dispose();
        }

        public Session Get(long id)
        {
            Session session;
            this.sessions.TryGetValue(id, out session);
            return session;
        }

        public virtual Session Create(IPEndPoint ipEndPoint)
        {
            try
            {
                AClient client = this.Server.ConnectClient(ipEndPoint);
                Session session = ObjectFactory.Create<Session, NetworkComponent, AClient>(this, client);
                session.Network = this;
                client.ErrorCallback += (c, e) => { this.Remove(session.id); };
                this.sessions.Add(session.id, session);
                return session;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Update()
        {
            if(this.Server == null)
            {
                return;
            }
            this.Server.Update();
        }

        public override void Dispose()
        {
            if(this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            foreach (Session session in this.sessions.Values.ToArray())
            {
                session.Dispose();
            }

            this.Server.Dispose();
        }
    }
}
