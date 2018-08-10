using System;
using System.Threading.Tasks;

namespace GN
{
    public class OuterMessageDispatcher : IMessageDispatcher
    {
        public async void Dispatch(Session session, Packet packet)
        {
#if Server
            ushort opcode = packet.Opcode;
            uint rpcId = packet.RpcId;

            Type messageType = (session.Network.Parent as Entity).GetComponent<OpcodeTypeComponent>().GetRequestType(opcode);
            object message = session.Network.MessagePacker.DeserializeFrom(messageType, packet.Bytes, Packet.Index, packet.Length);
            if (message != null)
            {
                var actor = session.GetComponent<RedirectActorIdComponent>();
                if (actor == null)
                {
                    //不需要转发
                    await Game.Scene.GetComponent<MessageDispatherComponent>().Handle(session, new MessageInfo(opcode, rpcId, session, message));
                }
                else          
                {
                    //命令转发
                    var actorMessageSender = Game.Scene.GetComponent<ActorMessageSenderComponent>().Get(actor.ActorId);
                    if (rpcId > 0)
                    {
                        var response = await actorMessageSender.Call(message as IRequest);
                        if (response == null)
                        {
                            session.Reply(rpcId, new MessageResponse() { Error = ErrorCode.ERR_RpcFail, Message = "Rpc Invoke Failed" });
                        }
                        else
                        {
                            session.Reply(rpcId, response);
                        }
                    }
                    else
                    {
                        actorMessageSender.Send(message as IRequest);
                    }
                }
            }
#else
            await Task.CompletedTask;
#endif
        }
    }
}