using System;

namespace GN
{
    public class OuterMessageDispatcher : IMessageDispatcher
    {
        public async void Dispatch(Session session, Packet packet)
        {
            ushort opcode = packet.Opcode;
            uint rpcId = packet.RpcId;

            Type messageType = (session.Network.Parent as Entity).GetComponent<OpcodeTypeComponent>().GetRequestType(opcode);
            object message = session.Network.MessagePacker.DeserializeFrom(messageType, packet.Bytes, Packet.Index, packet.Length);
            if (message != null)
            {
                var messageDispather = Game.Scene.GetComponent<MessageDispatherComponent>();
                if (messageDispather.IsLocalHandler(opcode))    //命令是否在本进程
                {
                    await Game.Scene.GetComponent<MessageDispatherComponent>().Handle(session, new MessageInfo(opcode, rpcId, session, message));
                }
                else          //命令转发
                {
                    var actor = session.GetComponent<ActorIdComponent>();
                    int appId = IdGenerater.GetAppIdFromId(actor.ActorId);
                    var appInfo = await Game.Scene.GetComponent<SlaveComponent>().Get(appId);
                    (message as IRequest).ActorId = actor.ActorId;
                    var innerSession = Game.Scene.GetComponent<NetInnerComponent>().Get(appInfo.innerAddress.IpEndPoint());
                    if (rpcId > 0)
                    {
                        var response = await innerSession.Call(message as IRequest);
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
                        innerSession.Send(message as IRequest);
                    }
                }
            }
        }
    }
}
