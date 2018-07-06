using System.Threading.Tasks;
using GN;

namespace Logic
{
    [MessageHandler(AppType.Gate)]
    public class GateLoginHandler : AMRpcHandler<GateLogin.Request, GateLogin.Response>
    {
        protected override Task<GateLogin.Response> Run(Session session, GateLogin.Request message)
        {
            var resp = new GateLogin.Response();
            var account =  Game.Scene.GetComponent<GateSessionKeyComponent>().Get(message.key);
            if(account == message.account)
            {
                resp.twill_user = new Twill_User()
                {
                };
            }
            else
            {
                resp.Message = "GateLogin Failed";
            }
            return Task.FromResult(resp);
        }
    }
}
