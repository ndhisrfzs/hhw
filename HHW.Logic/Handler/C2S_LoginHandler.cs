using HHW.Service;
using System;
using System.Threading.Tasks;

namespace HHW.Logic
{
    [MessageHandler(AppType.Login)]
    public class C2S_LoginHandler : AMRpcHandler<C2S_Login, S2C_Login>
    {
        protected override Task<S2C_Login> Run(Session session, C2S_Login message)//, Action<S2C_Login> reply)
        {
            S2C_Login response = new S2C_Login();
            Console.WriteLine("C2S_LoginHandler " + message.Account);
            response.IsLogin = true;
            //reply(response);
            return Task.FromResult(response);
        }
    }
}
