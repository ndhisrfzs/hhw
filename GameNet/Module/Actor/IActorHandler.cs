using System.Threading.Tasks;

namespace GN
{
    public enum ActorHandlerType
    {
        Gate,
        Common
    }
    public interface IActorHandler
    {
        Task Handle(Session session, MessageInfo messageInfo); 
    }
}
