namespace GN
{
    public interface IMessage
    {
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
    }

    public class MessageResponse : IResponse
    {
        public short Error { get; set; }
        public string Message { get; set; }
    }
}
