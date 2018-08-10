using GN;
using System.Threading.Tasks;

namespace Logic
{
    [MessageHandler(AppType.Gate)]
    public class M2G_MatchSuccessHandler : AMRpcHandler<Session, M2G_MatchSuccess.Request, M2G_MatchSuccess.Response>
    {
        protected override Task<M2G_MatchSuccess.Response> Run(Session entity, M2G_MatchSuccess.Request message)
        {
            M2G_MatchSuccess.Response resp = new M2G_MatchSuccess.Response();
            var session = Game.Scene.GetComponent<NetOuterComponent>().Get(message.uid);
            if(session != null)
            {
                session.AddComponent<ActorComponent, ActorHandlerType>(ActorHandlerType.Gate);
                session.AddComponent<RedirectActorIdComponent, long>(message.roomActorId);
                session.Send(new RoomInitSuccess.Request());
            }

            return Task.FromResult(resp);
        }
    }
}
