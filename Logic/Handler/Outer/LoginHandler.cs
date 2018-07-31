using GN;
using System;
using System.Threading.Tasks;

namespace Logic
{
    [MessageHandler(AppType.Login)]
    public class LoginHandler : AMRpcHandler<Session, Login.Request, Login.Response>
    {
        protected override async Task<Login.Response> Run(Session session, Login.Request message)
        {
            Login.Response response = new Login.Response();
            Console.WriteLine("C2S_LoginHandler " + message.Account);
            var slave = Game.Scene.GetComponent<SlaveComponent>();
            var gateAddress = await slave.Get(AppType.Gate);
            var gateSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gateAddress.innerAddress.IpEndPoint());
            var result = (GetLoginKey.Response)await gateSession.Call(new GetLoginKey.Request() { account = message.Account });
            response.IsLogin = true;
            response.Key = result.key;
            response.GateIP = gateAddress.outerAdderss.IP;
            response.GatePort = gateAddress.outerAdderss.Port;
            return response;
        }
    }
}
