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
                var config = Game.Scene.GetComponent<ConfigComponent>();
                if (config.AppType.Is(AppType.Gate))    // 判断当前是否gate
                {
                    var messageDispather = Game.Scene.GetComponent<MessageDispatherComponent>();
                    var type = messageDispather.GetOpcodeApp(opcode);
                    if (type.Is(config.AppType))    //命令是否在本app
                    {
                        Game.Scene.GetComponent<MessageDispatherComponent>().Handle(session, new MessageInfo(opcode, rpcId, message));
                    }
                    else          //命令转发
                    {
                        var playerSessionManager = session.GetComponent<PlayerSessionManagerComponent>();
                        var response = await playerSessionManager.Call(type, message);
                        if (response == null)
                        {
                            session.Reply(rpcId, new MessageResponse() { Error = ErrorCode.ERR_RpcFail, Message = "Rpc Invoke Failed" });
                        }
                        else
                        {
                            session.Reply(rpcId, response);
                        }
                    }
                }
                else             //非Gate命令不会转发
                {
                    Game.Scene.GetComponent<MessageDispatherComponent>().Handle(session, new MessageInfo(opcode, rpcId, message));
                }
            }
        }
    }
}
