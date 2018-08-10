using System;
using System.Threading.Tasks;

namespace GN
{
    public class GateActorHandler : IActorHandler
    {
        public async Task Handle(Session session, MessageInfo messageInfo)
        {
            MessageResponse resp = new MessageResponse();
            try
            {
                Session client = messageInfo.entity as Session;
                (messageInfo.Message as IRequest).ActorId = 0;
                client.Send(messageInfo.Message as IMessage);

                session.Reply(messageInfo.RpcId, resp);

                await Task.CompletedTask;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
