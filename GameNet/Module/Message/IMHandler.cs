using System;

namespace GN
{
    public interface IMHandler
    {
        void Handle(Session session, uint rpcId, object message);
        Type GetMessageType();
    }
}
