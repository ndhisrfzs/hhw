namespace GN
{
    public interface IMessage
    {
        uint RpcId { get; set; }
    }

    public interface IRequest : IMessage
    {
    }

    public interface IResponse : IMessage
    {
        short Error { get; set; }
        string Message { get; set; }
    }

    public class MessageRequest : IRequest
    {
        public uint RpcId { get; set; }
    }

    public class MessageResponse : IResponse
    {
        public uint RpcId { get; set; }
        public short Error { get; set; }
        public string Message { get; set; }
    }
}
