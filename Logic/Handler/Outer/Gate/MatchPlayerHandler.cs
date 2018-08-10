using System.Threading.Tasks;
using GN;

namespace Logic
{
    [MessageHandler(AppType.Gate)]
    public class MatchPlayerHandler : AMRpcHandler<Session, MatchPlayer.Request, MatchPlayer.Response>
    {
        protected override async Task<MatchPlayer.Response> Run(Session session, MatchPlayer.Request message)
        {
            MatchPlayer.Response resp = new MatchPlayer.Response();
            resp.isSuccess = true;

            var slave = Game.Scene.GetComponent<SlaveComponent>();
            var matchConfig = await slave.Get(AppType.Match);
            var matchSession = Game.Scene.GetComponent<NetInnerComponent>().Get(matchConfig.innerAddress.IpEndPoint());

            var user = session.GetComponent<UserComponent>().user;

            var matchResp = (G2M_MatchPlayer.Response) await matchSession.Call(new G2M_MatchPlayer.Request()
            {
                uid = user.uid,
                name = user.name,
                sex = user.sex,
                head_url = user.head_url,
                score = user.score,
                game = message.game,
                game_type = message.gameType,
                model_type = message.modelType
            });

            resp.isSuccess = matchResp.isSuccess;

            return resp;
        }
    }
}
