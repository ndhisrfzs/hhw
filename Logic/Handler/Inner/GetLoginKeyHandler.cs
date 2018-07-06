using System.Threading.Tasks;
using GN;

namespace Logic
{
    [MessageHandler(AppType.Gate)]
    public class GetLoginKeyHandler : AMRpcHandler<GetLoginKey.Request, GetLoginKey.Response>
    {
        protected override Task<GetLoginKey.Response> Run(Session session, GetLoginKey.Request message)
        {
            var resp = new GetLoginKey.Response();

            long key = IdGenerater.GenerateId();
            Game.Scene.GetComponent<GateSessionKeyComponent>().Add(key, message.account);
            resp.key = key;
            return Task.FromResult(resp);
        }
    }
}
