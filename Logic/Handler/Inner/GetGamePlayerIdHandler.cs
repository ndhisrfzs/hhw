using System.Threading.Tasks;
using GN;

namespace Logic
{
    [MessageHandler(AppType.Game)]
    public class GetGamePlayerIdHandler : AMRpcHandler<Session, GetGamePlayerId.Request, GetGamePlayerId.Response>
    {
        protected override Task<GetGamePlayerId.Response> Run(Session entity, GetGamePlayerId.Request message)
        {
            GetGamePlayerId.Response resp = new GetGamePlayerId.Response();
            GamePlayer player = ObjectFactory.CreateWithParent<GamePlayer>(entity);
            player.AddComponent<ActorComponent>();
            player.uid = message.sessionId;

            resp.actorId = player.id;

            return Task.FromResult(resp);
        }
    }
}
