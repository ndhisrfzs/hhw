namespace GN
{
    public interface IMessage
    {
    }

    public interface IRequest : IMessage
    {
        long ActorId { get; set; }
    }

    public interface IResponse : IMessage
    {
        short Error { get; set; }
        string Message { get; set; }
    }

    public class MessageRequest : IRequest
    {
        public long ActorId { get; set; }
    }

    public class MessageResponse : IResponse
    {
        public short Error { get; set; }
        public string Message { get; set; }
    }
}
