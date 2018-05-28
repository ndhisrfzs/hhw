using HHW.Service;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace HHW.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Game.Scene.AddComponent<OpcodeTypeComponent>();
            Game.Scene.AddComponent<MessageDispatherComponent, AppType>(AppType.AllServer);
            Game.Scene.AddComponent<NetInnerComponent, IPEndPoint>(new IPEndPoint(IPAddress.Any, 7777));
            Game.Scene.AddComponent<NetOuterComponent, IPEndPoint>(new IPEndPoint(IPAddress.Any, 6666));

            Game.Client.AddComponent<NetInnerComponent>();
            Game.Client.AddComponent<OpcodeTypeComponent>();
            var session = Game.Client.GetComponent<NetInnerComponent>().Create(NetworkHelper.ToIPEndPoint("127.0.0.1", 7777));
            Test(session);

            while (true)
            {
                try
                {
                    Thread.Sleep(1);
                    EventSystem.Update();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        //static void TT(Session session)
        //{
        //    var b = Test(session);
        //    Console.WriteLine(b.Result);
        //}

        static async void Test(Session session)
        {
            var result = (S2C_Login)await session.Call(new C2S_Login()
            {
                Account = "fzf",
                Password = "123456"
            });
            Console.WriteLine(result.IsLogin);
        }
    }
}
