using System;

namespace GN
{
    public class InnerMessageDispatcher : IMessageDispatcher
    {
        public async void Dispatch(Session session, Packet packet)
        {
            ushort opcode = packet.Opcode;
            uint rpcId = packet.RpcId;
            Type messageType = Game.Scene.GetComponent<OpcodeTypeComponent>().GetRequestType(opcode);
            object message = session.Network.MessagePacker.DeserializeFrom(messageType, packet.Bytes, Packet.Index, packet.Length);
            if((message as IRequest).ActorId > 0)
            {
                Entity entity = Game.EntityManager.Get((message as IRequest).ActorId);
                if(entity != null)
                {
                    (message as IRequest).ActorId = 0;
                    entity.GetComponent<ActorComponent>().Add(new ActorMessage() { rpcId = rpcId, opcode = opcode, session = session, message = message });
                }
            }

            await Game.Scene.GetComponent<MessageDispatherComponent>().Handle(session, new MessageInfo(opcode, rpcId, session, message));
        }
    }
}
