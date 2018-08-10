using System;

namespace GN
{
    public class ClientMessageDispatcher : IMessageDispatcher
    {
        public async void Dispatch(Session session, Packet packet)
        {
            ushort opcode = packet.Opcode;
            uint rpcId = packet.RpcId;

            Type messageType = (session.Network.Parent as Entity).GetComponent<OpcodeTypeComponent>().GetRequestType(opcode);
            object message = session.Network.MessagePacker.DeserializeFrom(messageType, packet.Bytes, Packet.Index, packet.Length);

            await Game.Scene.GetComponent<MessageDispatherComponent>().Handle(session, new MessageInfo(opcode, rpcId, session, message));
        }
    }
}
