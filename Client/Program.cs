using GN;
using Logic;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HHW.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Game.EventSystem.Add(DLLType.Model, typeof(Game).Assembly);
            Game.EventSystem.Add(DLLType.Logic, typeof(OuterOpcode).Assembly);

            Game.Scene.AddComponent<OpcodeTypeComponent>();
            Game.Scene.AddComponent<NetOuterComponent>();

            var session = Game.Scene.GetComponent<NetOuterComponent>().Create(NetworkHelper.ToIPEndPoint("127.0.0.1", 7777));
            Test(session);

            while (true)
            {
                try
                {
                    Thread.Sleep(1);
                    Game.EventSystem.Update();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        static async void Test(Session session)
        {
            var result = (Login.Response)await session.Call(new Login.Request()
            {
                Account = "fzf",
                Password = "123456"
            });
            Console.WriteLine(result.IsLogin);
            Ping(session);
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
