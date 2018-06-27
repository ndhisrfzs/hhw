using System;

namespace HHW.Service
{
    public class OuterMessageDispatcher : IMessageDispatcher
    {
        public void Dispatch(Session session, Packet packet)
        {
            ushort opcode = packet.Opcode;
            Type messageType = (session.Network.Parent as Entity).GetComponent<OpcodeTypeComponent>().GetType(opcode);
            object message = session.Network.MessagePacker.DeserializeFrom(messageType, packet.Bytes, Packet.Index, packet.Length);

            if(message != null)
            {
                Game.Scene.GetComponent<MessageDispatherComponent>().Handle(session, new MessageInfo(opcode, message));
            }
        }
    }
}
