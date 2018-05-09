namespace HHW.Service
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
}
