#if Server
using System.Threading.Tasks;

namespace GN
{
    public class SlaveComponent : Component
    {
        public async void Awake()
        {
            var config = Game.Scene.GetComponent<ConfigComponent>();
            var info = new AppInfo() { appId = config.AppId, appType = config.AppType, innerAddress = config.InnerAddress, outerAdderss = config.OuterAddress };
            if (config.AppType.Is(AppType.Master))
            {
                var master = Game.Scene.GetComponent<MasterComponent>();
                if (!master.Add(info))
                {
                    Log.Error("Register Appinfo Error");
                }
            }
            else
            {
                var session = Game.Scene.GetComponent<NetInnerComponent>().Get(config.MasterAddress.IpEndPoint());
                var result = (RegisterApp.Response)await session.Call(new RegisterApp.Request(){ appInfo = info });
                if (!result.isSuccess)
                {
                    Log.Error("Register Appinfo Error");
                }
            }
        }

        public async Task<AppInfo> Get(AppType appType)
        {
            var config = Game.Scene.GetComponent<ConfigComponent>();
            if (config.AppType.Is(AppType.Master))
            {
                var master = Game.Scene.GetComponent<MasterComponent>();
                return master.Get(appType);
            }
            else
            {
                var session = Game.Scene.GetComponent<NetInnerComponent>().Get(config.MasterAddress.IpEndPoint());
                var result = (GetAppByType.Response)await session.Call(new GetAppByType.Request() { appType = appType });
                return result.appInfo;
            }
        }

        public async Task<AppInfo> Get(int appId)
        {
            var config = Game.Scene.GetComponent<ConfigComponent>();
            if (config.AppType.Is(AppType.Master))
            {
                var master = Game.Scene.GetComponent<MasterComponent>();
                return master.Get(appId);
            }
            else
            {
                var session = Game.Scene.GetComponent<NetInnerComponent>().Get(config.MasterAddress.IpEndPoint());
                var result = (GetAppById.Response)await session.Call(new GetAppById.Request() { appId = appId });
                return result.appInfo;
            }
        }
    }
}
#endif