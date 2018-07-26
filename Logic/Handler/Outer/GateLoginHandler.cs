using System.Threading;
using System.Threading.Tasks;
using GN;

namespace Logic
{
    [MessageHandler(AppType.Gate)]
    public class GateLoginHandler : AMRpcHandler<GateLogin.Request, GateLogin.Response>
    {
        private static int uids = 0;
        protected override async Task<GateLogin.Response> Run(Session session, GateLogin.Request message)
        {
            var resp = new GateLogin.Response();
            var account =  Game.Scene.GetComponent<GateSessionKeyComponent>().Get(message.key);
            if(account == message.account)
            {
                var slave = Game.Scene.GetComponent<SlaveComponent>();
                var gameConfig = await slave.Get(AppType.Game);
                var gameSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gameConfig.innerAddress.IpEndPoint());
                var playerSessionManager = session.AddComponent<PlayerSessionManagerComponent>();
                playerSessionManager.RegisterConnectedApps(AppType.Game, gameSession);

                resp.twill_user = new Twill_User()
                {
                    uid = Interlocked.Increment(ref uids),
                    name = "玩家" + account.Substring(account.Length - 3),
                    gold = 100
                };
            }
            else
            {
                resp.Message = "GateLogin Failed";
            }
            return resp;
        }
    }
}
