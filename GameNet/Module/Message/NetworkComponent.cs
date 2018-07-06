using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GN
{
    public abstract class NetworkComponent : Component
    {
        private AServer Server;

        private readonly Dictionary<long, Session> sessions = new Dictionary<long, Session>();

        public IMessagePacker MessagePacker { get; set; }
        public IMessageDispatcher MessageDispatcher { get; set; }

        public void Awake()
        {
            this.Server = new TServer();
            this.Server.AcceptCallback += this.OnAccept;
        }

        public void Awake(IPEndPoint ipEndPoint)
        {
            this.Server = new TServer(ipEndPoint);
            this.Server.AcceptCallback += this.OnAccept;
        }

        public void Start()
        {
            this.Server.Start();
        }

        public void OnAccept(AClient client)
        {
            Session session = ObjectFactory.CreateWithParent<Session, AClient>(this, client);
            this.sessions.Add(session.id, session);
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
                Session session = ObjectFactory.CreateWithParent<Session, AClient>(this, client);
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
