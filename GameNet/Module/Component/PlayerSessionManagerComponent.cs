using System.Collections.Generic;
using System.Threading.Tasks;

namespace GN
{
    public class PlayerSessionManagerComponent : Component
    {
        private readonly Dictionary<AppType, Session> playerConnectedApps = new Dictionary<AppType, Session>();

        public void RegisterConnectedApps(AppType appType, Session session)
        {
            if(playerConnectedApps.ContainsKey(appType))
            {
                playerConnectedApps[appType] = session;
            }
            else
            {
                playerConnectedApps.Add(appType, session);
            }
        }

        public async Task<IResponse> Call(AppType appType, object message)
        {
            Session session;
            if(playerConnectedApps.TryGetValue(appType, out session))
            {
                return await session.Call(message as IRequest);
            }

            return null;
        }
    }
}
