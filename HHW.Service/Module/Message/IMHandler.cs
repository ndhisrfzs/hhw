using System;

namespace HHW.Service
{
    public interface IMHandler
    {
        void Handle(Session session, object message);
        Type GetMessageType();
    }
}
