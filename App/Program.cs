using GN;
using Logic;
using NLog;
using System;
using System.Net;
using System.Threading;

namespace HHW.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Game.EventSystem.Add(DLLType.Model, typeof(Game).Assembly);
            Game.EventSystem.Add(DLLType.Logic, DllHelper.GetLogicAssembly());

            var config = Game.Scene.AddComponent<ConfigComponent, string>("Config");

            Game.Scene.AddComponent<OpcodeTypeComponent>();
            Game.Scene.AddComponent<MessageDispatherComponent, AppType>(config.AppType);
            Game.Scene.AddComponent<TimerComponent>();
            Game.Scene.AddComponent<ActorManagerComponent>();
            Game.Scene.AddComponent<ActorMessageSenderComponent>();

            switch(config.AppType)
            {
                case AppType.Master:
                    {
                        Game.Scene.AddComponent<MasterComponent>();
                        Game.Scene.AddComponent<NetInnerComponent, IPEndPoint>(new IPEndPoint(IPAddress.Any, config.InnerAddress.Port));
                    }
                    break;
                case AppType.Login:
                    {
                        Game.Scene.AddComponent<NetInnerComponent, IPEndPoint>(new IPEndPoint(IPAddress.Any, config.InnerAddress.Port));
                        Game.Scene.AddComponent<NetOuterComponent, IPEndPoint>(new IPEndPoint(IPAddress.Any, config.OuterAddress.Port));
                        Game.Scene.AddComponent<SlaveComponent>();
                    }
                    break;
                case AppType.Gate:
                    {
                        Game.Scene.AddComponent<GateSessionKeyComponent>();
                        Game.Scene.AddComponent<NetInnerComponent, IPEndPoint>(new IPEndPoint(IPAddress.Any, config.InnerAddress.Port));
                        Game.Scene.AddComponent<NetInnerComponent, IPEndPoint>(new IPEndPoint(IPAddress.Any, config.InnerAddress.Port));
                        Game.Scene.AddComponent<SlaveComponent>();
                    }
                    break;
                case AppType.DB:
                    {
                        Game.Scene.AddComponent<NetInnerComponent, IPEndPoint>(new IPEndPoint(IPAddress.Any, config.InnerAddress.Port));
                        Game.Scene.AddComponent<SlaveComponent>();
                    }
                    break;
                case AppType.Game:
                    {
                        Game.Scene.AddComponent<NetInnerComponent, IPEndPoint>(new IPEndPoint(IPAddress.Any, config.InnerAddress.Port));
                        Game.Scene.AddComponent<SlaveComponent>();
                        Game.Scene.AddComponent<RandomComponent>();
                        Game.Scene.AddComponent<RoomKeyComponent, int>(899999);
                        Game.Scene.AddComponent<RoomManagerComponent>();
                    }
                    break;
                case AppType.AllServer:
                    {
                        Game.Scene.AddComponent<MasterComponent>();
                        Game.Scene.AddComponent<GateSessionKeyComponent>();
                        Game.Scene.AddComponent<NetInnerComponent, IPEndPoint>(new IPEndPoint(IPAddress.Any, config.InnerAddress.Port));
                        Game.Scene.AddComponent<NetOuterComponent, IPEndPoint>(new IPEndPoint(IPAddress.Any, config.OuterAddress.Port));
                        Game.Scene.AddComponent<DBModelTypeComponent>();
                        Game.Scene.AddComponent<DBProxyComponent>();
                        Game.Scene.AddComponent<DBComponent, string>(config.DbConnection);
                        Game.Scene.AddComponent<SlaveComponent>();
                        Game.Scene.AddComponent<RandomComponent>();
                        Game.Scene.AddComponent<RoomKeyComponent, int>(899999);
                        Game.Scene.AddComponent<RoomManagerComponent>();
                    }
                    break;
            }

            //var dbProxy = Game.Scene.GetComponent<DBProxyComponent>();
            //var a = dbProxy.QueryFirstOrDefault<Twill_User>("");

            LogManager.Configuration.Variables["appType"] = config.AppType.ToString();
            LogManager.Configuration.Variables["appId"] = config.AppId.ToString();
            LogManager.Configuration.Variables["appTypeFormat"] = $"{config.AppType, -8}";
            LogManager.Configuration.Variables["appIdFormat"] = $"{config.AppId:D3}";
            Log.Info("Start Over");

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
    }
}
