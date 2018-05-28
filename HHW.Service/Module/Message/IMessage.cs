namespace HHW.Service
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

    public class Request : IRequest
    {
        public uint RpcId { get; set; }
    }

    public class Response : IResponse
    {
        public uint RpcId { get; set; }
        public short Error { get; set; }
        public string Message { get; set; }
    }
}
