using System.Threading;
using System.Threading.Tasks;
using GN;

namespace Logic
{
    [MessageHandler(AppType.Gate)]
    public class GateLoginHandler : AMRpcHandler<Session, GateLogin.Request, GateLogin.Response>
    {
        protected override async Task<GateLogin.Response> Run(Session session, GateLogin.Request message)
        {
            var resp = new GateLogin.Response();
            var account =  Game.Scene.GetComponent<GateSessionKeyComponent>().Get(message.key);
            if(account == message.account)
            {
                var slave = Game.Scene.GetComponent<SlaveComponent>();
                var gameConfig = await slave.Get(AppType.Game);
                var gameSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gameConfig.innerAddress.IpEndPoint());

                var gamePlayerId =(GetGamePlayerId.Response)await gameSession.Call(new GetGamePlayerId.Request() { });
                session.AddComponent<ActorIdComponent, long>(gamePlayerId.id);

                resp.twill_user = new Twill_User()
                {
                    uid = gamePlayerId.id,
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
