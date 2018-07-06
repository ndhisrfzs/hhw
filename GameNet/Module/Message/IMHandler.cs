using System;

namespace GN
{
    public interface IMHandler
    {
        void Handle(Session session, object message);
        Type GetMessageType();
    }
}
