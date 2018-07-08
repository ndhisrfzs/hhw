using System.Threading.Tasks;

namespace GN
{
    [MessageHandler(AppType.Master)]
    public class RegisterAppHandler : AMRpcHandler<RegisterApp.Request, RegisterApp.Response>
    {
        protected override Task<RegisterApp.Response> Run(Session session, RegisterApp.Request message)
        {
            var resp = new RegisterApp.Response();
            var master = Game.Scene.GetComponent<MasterComponent>();
            resp.IsSuccess = master.Add(message.appInfo);
            if(resp.IsSuccess)
            {
                session.AddComponent<SlaveLifeComponent, int>(message.appInfo.AppId);
            }
            return Task.FromResult(resp);
        }
    }

    [MessageHandler(AppType.Master)]
    public class GetAppByIdHandlder : AMRpcHandler<GetAppById.Request, GetAppById.Response>
    {
        protected override Task<GetAppById.Response> Run(Session session, GetAppById.Request message)
        {
            var resp = new GetAppById.Response();
            var manager = Game.Scene.GetComponent<MasterComponent>();
            resp.appInfo = manager.Get(message.appId);
            return Task.FromResult(resp);
        }
    }

    [MessageHandler(AppType.Master)]
    public class GetAppByTypeHandlder : AMRpcHandler<GetAppByType.Request, GetAppByType.Response>
    {
        protected override Task<GetAppByType.Response> Run(Session session, GetAppByType.Request message)
        {
            var resp = new GetAppByType.Response();
            var manager = Game.Scene.GetComponent<MasterComponent>();
            resp.appInfo = manager.Get(message.appType);
            return Task.FromResult(resp);
        }
    }
}
