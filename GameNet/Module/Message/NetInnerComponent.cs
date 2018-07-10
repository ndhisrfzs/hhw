using System.Collections.Generic;
using System.Net;

namespace GN
{
    public class NetInnerComponent : NetworkComponent
    {
        public readonly Dictionary<IPEndPoint, Session> addressSessions = new Dictionary<IPEndPoint, Session>();

        public NetInnerComponent()
        {
            MessagePacker = new BsonPacker();
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
            Session session = null;
            if(this.addressSessions.TryGetValue(ipEndPoint, out session))
            {
                return session;
            }

            session = this.Create(ipEndPoint);

            this.addressSessions.Add(ipEndPoint, session);
            return session;
        }

       
    }
}
