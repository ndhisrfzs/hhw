using System;
using System.Threading.Tasks;

namespace GN
{
    public interface IMHandler
    {
        Task Handle(Session session, Entity entity, uint rpcId, object message);
        Type GetMessageType();
    }
}
