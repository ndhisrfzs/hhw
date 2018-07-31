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
            GamePlayer player = ObjectFactory.Create<GamePlayer>();
            player.AddComponent<ActorComponent>();

            resp.id = player.id;

            return Task.FromResult(resp);
        }
    }
}
