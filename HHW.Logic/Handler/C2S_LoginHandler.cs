using HHW.Service;
using System;

namespace HHW.Logic
{
    public class C2S_LoginHandler : AMRpcHandler<C2S_Login, S2C_Login>
    {
        protected override void Run(Session session, C2S_Login message, Action<S2C_Login> reply)
        {
            S2C_Login response = new S2C_Login();
            Console.WriteLine("C2S_LoginHandler " + message.Account);
            response.IsLogin = true;
            reply(response);
        }
    }
}
