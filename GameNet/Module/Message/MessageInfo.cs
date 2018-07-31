namespace GN
{
    public class MessageInfo
    {
        public ushort Opcode { get; }
        public uint RpcId { get; }
        public Entity entity { get; }
        public object Message { get; }

        public MessageInfo(ushort opcode, uint rpcId, Entity entity, object message)
        {
            this.Opcode = opcode;
            this.RpcId = rpcId;
            this.entity = entity;
            this.Message = message;
        }
    }
}
