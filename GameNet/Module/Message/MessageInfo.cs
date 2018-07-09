namespace GN
{
    public class MessageInfo
    {
        public ushort Opcode { get; }
        public uint RpcId { get; }
        public object Message { get; }

        public MessageInfo(ushort opcode, uint rpcId, object message)
        {
            this.Opcode = opcode;
            this.RpcId = rpcId;
            this.Message = message;
        }
    }
}
