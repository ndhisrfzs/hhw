﻿using System.Collections.Generic;
using System.Net;

namespace HHW.Service
{
    public class NetInnerComponent : NetworkComponent
    {
        public readonly Dictionary<IPEndPoint, Session> addressSessions = new Dictionary<IPEndPoint, Session>();

        public NetInnerComponent()
        {
            MessagePacker = new MessagePakcer();
            MessageDispatcher = new InnerMessageDispatcher();
        }

        public override void Remove(long id)
        {
            Session session = this.Get(id);
            if(session == null)
            {
                return;
            }

            this.addressSessions.Remove(session.RemoteAddress);

            base.Remove(id);
        }

        public Session Get(IPEndPoint ipEndPoint)
        {
            if(this.addressSessions.TryGetValue(ipEndPoint, out Session session))
            {
                return session;
            }

            session = this.Create(ipEndPoint);

            this.addressSessions.Add(ipEndPoint, session);
            return session;
        }

       
    }
}