namespace HHW.Service
{
    public interface IMessage
    {
        int RpcId { get; set; }
    }

    public interface IRequest : IMessage
    {
    }

    public interface IResponse : IMessage
    {
        int Error { get; set; }
        string Message { get; set; }
    }

    public class RequestMessage : IRequest
    {
        public int RpcId { get; set; }
    }

    public class ResponseMessage : IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public int RpcId { get; set; }
    }
}
