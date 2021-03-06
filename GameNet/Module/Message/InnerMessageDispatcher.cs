﻿using System;
using System.Threading.Tasks;

namespace GN
{
    public class InnerMessageDispatcher : IMessageDispatcher
    {
        public async void Dispatch(Session session, Packet packet)
        {
#if Server
            ushort opcode = packet.Opcode;
            uint rpcId = packet.RpcId;
            Type messageType = Game.Scene.GetComponent<OpcodeTypeComponent>().GetRequestType(opcode);
            object message = session.Network.MessagePacker.DeserializeFrom(messageType, packet.Bytes, Packet.Index, packet.Length);
            if((message as IRequest).ActorId > 0)
            {
                Entity entity = Game.Scene.GetComponent<ActorManagerComponent>().GetActor((message as IRequest).ActorId);
                if(entity != null)
                {
                    (message as IRequest).ActorId = 0;
                    entity.GetComponent<ActorComponent>().Add(new ActorMessage() { rpcId = rpcId, opcode = opcode, session = session, message = message });
                    return;
                }
            }

            await Game.Scene.GetComponent<MessageDispatherComponent>().Handle(session, new MessageInfo(opcode, rpcId, session, message));
#else
            await Task.CompletedTask;
#endif
        }
    }
}