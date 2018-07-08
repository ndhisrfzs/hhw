using System.Net;
using System.Threading.Tasks;

namespace GN
{
    public class DBProxyComponent : Component
    {
        private IPEndPoint dbAddress;
        public void Awake()
        {
            var config = Game.Scene.GetComponent<ConfigComponent>();
            dbAddress = config.DbAddress.IpEndPoint();
        }

        public async Task<T> QueryFirstOrDefault<T>(string filter)
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(dbAddress);
            return (T)await session.Call(new DBQueryFirst.Request() { CollectionName = typeof(T).Name, Filter = filter });
        }
    }
}
