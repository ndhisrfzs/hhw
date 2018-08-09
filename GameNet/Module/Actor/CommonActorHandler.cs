using System.Threading.Tasks;

namespace GN
{
    public class CommonActorHandler : IActorHandler
    {
        public async Task Handle(Session session, MessageInfo messageInfo)
        {
            await Game.Scene.GetComponent<MessageDispatherComponent>().Handle(session, messageInfo);

            if(!Game.Scene.GetComponent<OpcodeTypeComponent>().HasResponse(messageInfo.Opcode))
            {
                //没有返回值的调用强制给个回复
                MessageResponse resp = new MessageResponse();
                session.Reply(messageInfo.RpcId, resp);
            }
        }
    }
}
