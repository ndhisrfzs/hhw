using GN;
using System;
using System.Threading.Tasks;

namespace Logic
{
    [MessageHandler(AppType.Match)]
    public class G2M_MatchPlayerHandler : AMRpcHandler<Session, G2M_MatchPlayer.Request, G2M_MatchPlayer.Response>
    {
        protected override Task<G2M_MatchPlayer.Response> Run(Session entity, G2M_MatchPlayer.Request message)
        {
            G2M_MatchPlayer.Response resp = new G2M_MatchPlayer.Response();
            var matchComponent = Game.Scene.GetComponent<MatchComponent>();
            resp.isSuccess = matchComponent.WaitMatch(new Matcher() {
                uid = message.uid,
                name = message.name,
                sex = message.sex,
                head_url = message.head_url,
                score = message.score,
                game = message.game,
                game_type = message.game_type,
                model_type = message.model_type
            });
            return Task.FromResult(resp);
        }
    }
}
