using System.Threading;
using System.Threading.Tasks;
using GN;

namespace Logic
{
    [MessageHandler(AppType.Gate)]
    public class GateLoginHandler : AMRpcHandler<Session, GateLogin.Request, GateLogin.Response>
    {
        protected override Task<GateLogin.Response> Run(Session session, GateLogin.Request message)
        {
            var resp = new GateLogin.Response();
            var account =  Game.Scene.GetComponent<GateSessionKeyComponent>().Get(message.key);
            if(account == message.account)
            {
                //var slave = Game.Scene.GetComponent<SlaveComponent>();
                //var gameConfig = await slave.Get(AppType.Game);
                //var gameSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gameConfig.innerAddress.IpEndPoint());

                //var gamePlayerId =(GetGamePlayerId.Response)await gameSession.Call(new GetGamePlayerId.Request() { sessionId = session.id });
                //session.AddComponent<RedirectActorIdComponent, long>(gamePlayerId.actorId);
                //session.AddComponent<ActorComponent, ActorHandlerType>(ActorHandlerType.Gate);

                var userComponent = session.AddComponent<UserComponent>();

                userComponent.user = new Twill_User()
                {
                    uid = session.id,
                    name = "玩家" + account.Substring(account.Length - 3),
                    sex = 1,
                    head_url = null,
                    gold = 100
                };
                var appId = IdGenerater.GetAppIdFromId(session.id);

                resp.twill_user = userComponent.user;
            }
            else
            {
                resp.Message = "GateLogin Failed";
            }
            return Task.FromResult(resp);
        }
    }
}
