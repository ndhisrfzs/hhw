using HHW.Service;
using System;
using System.Threading.Tasks;

namespace HHW.Logic
{
    [MessageHandler(AppType.Login)]
    public class LoginHandler : AMRpcHandler<Login.Request, Login.Response>
    {
        protected override Task<Login.Response> Run(Session session, Login.Request message)
        {
            Login.Response response = new Login.Response();
            Console.WriteLine("C2S_LoginHandler " + message.Account);
            response.IsLogin = true;
            //reply(response);
            return Task.FromResult(response);
        }
    }
}
