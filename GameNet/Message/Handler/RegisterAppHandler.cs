using System.Threading.Tasks;

namespace GN
{
    [MessageHandler(AppType.Master)]
    public class RegisterAppHandler : AMRpcHandler<AppInfo, RegisterAppResponse>
    {
        protected override Task<RegisterAppResponse> Run(Session session, AppInfo message)
        {
            var resp = new RegisterAppResponse();
            var master = Game.Scene.GetComponent<MasterComponent>();
            resp.IsSuccess = master.Add(message);
            if(resp.IsSuccess)
            {
                session.AddComponent<SlaveLifeComponent, int>(message.AppId);
            }
            return Task.FromResult(resp);
        }
    }

    [MessageHandler(AppType.Master)]
    public class GetAppByIdHandlder : AMRpcHandler<GetAppById, ResponseAppInfo>
    {
        protected override Task<ResponseAppInfo> Run(Session session, GetAppById message)
        {
            var resp = new ResponseAppInfo();
            var manager = Game.Scene.GetComponent<MasterComponent>();
            resp.appInfo = manager.Get(message.appId);
            return Task.FromResult(resp);
        }
    }

    [MessageHandler(AppType.Master)]
    public class GetAppByTypeHandlder : AMRpcHandler<GetAppByType, ResponseAppInfo>
    {
        protected override Task<ResponseAppInfo> Run(Session session, GetAppByType message)
        {
            var resp = new ResponseAppInfo();
            var manager = Game.Scene.GetComponent<MasterComponent>();
            resp.appInfo = manager.Get(message.appType);
            return Task.FromResult(resp);
        }
    }
}
