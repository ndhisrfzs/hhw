namespace GN
{
    public interface IActorMessage : IRequest
    {
        long ActorId { get; set; }
    }

    public interface IActorRequest : IActorMessage
    {

    }

    public interface IActorResponse : IResponse
    {

    }

    public class ActorRequest : IActorRequest
    {
        public uint RpcId { get; set; }
        public long ActorId { get; set; }
    }

    public class ActorResponse : IActorResponse
    {
        public uint RpcId { get; set; }
        public short Error { get; set; }
        public string Message { get; set; } 
    }
}
