using System.Threading.Tasks;

namespace GN
{
    [MessageHandler(AppType.Master)]
    public class RegisterAppHandler : AMRpcHandler<Session, RegisterApp.Request, RegisterApp.Response>
    {
        protected override Task<RegisterApp.Response> Run(Session session, RegisterApp.Request message)
        {
            var resp = new RegisterApp.Response();
            var master = Game.Scene.GetComponent<MasterComponent>();
            resp.isSuccess = master.Add(message.appInfo);
            if(resp.isSuccess)
            {
                session.AddComponent<SlaveLifeComponent, int>(message.appInfo.appId);
            }
            return Task.FromResult(resp);
        }
    }

    [MessageHandler(AppType.Master)]
    public class GetAppByIdHandlder : AMRpcHandler<Session, GetAppById.Request, GetAppById.Response>
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
    public class GetAppByTypeHandlder : AMRpcHandler<Session, GetAppByType.Request, GetAppByType.Response>
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
