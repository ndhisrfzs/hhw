using System.Threading.Tasks;
using GN;

namespace Logic
{
    [MessageHandler(AppType.Game)]
    public class MatchPlayerHandler : AMRpcHandler<GamePlayer, MatchPlayer.Request, MatchPlayer.Response>
    {
        protected override Task<MatchPlayer.Response> Run(GamePlayer player, MatchPlayer.Request message)
        {
            MatchPlayer.Response resp = new MatchPlayer.Response();
            resp.isSuccess = true;

            return Task.FromResult(resp);
        }
    }
}
