using HHW.Logic;
using HHW.Service;
using NLog;
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

            LogManager.Configuration.Variables["appType"] = "Server";
            LogManager.Configuration.Variables["appId"] = "1";
            LogManager.Configuration.Variables["appTypeFormat"] = "Server";
            LogManager.Configuration.Variables["appIdFormat"] = "001";
            //TestClass tc = new TestClass();
            //int i = tc.Test();
            Test(session);
            Ping(session);
            Log.Info("Start Over");

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
            var result = (Login.Response)await session.Call(new Login.Request()
            {
                Account = "fzf",
                Password = "123456"
            });
            Console.WriteLine(result.IsLogin);
        }

        static async void Ping(Session session)
        {
            while(true)
            {
                await Task.Delay(1000);
                session.Send(new Ping() { });
            }
        }
    }
}
