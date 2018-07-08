using System;

namespace GN
{
    public class InnerMessageDispatcher : IMessageDispatcher
    {
        public void Dispatch(Session session, Packet packet)
        {
            ushort opcode = packet.Opcode;
            Type messageType = Game.Scene.GetComponent<OpcodeTypeComponent>().GetRequestType(opcode);
            object message = session.Network.MessagePacker.DeserializeFrom(messageType, packet.Bytes, Packet.Index, packet.Length);

            //if(message is IActorMessage iActorMessage)
            //{
            //    Entity entity = Game.Scene.GetComponent<ActorManagerComponent>().Get(iActorMessage.ActorId);
            //    if(entity == null)
            //    {
            //        session.Reply(response);
            //    }
            //}

            Game.Scene.GetComponent<MessageDispatherComponent>().Handle(session, new MessageInfo(opcode, message));
        }
    }
}
