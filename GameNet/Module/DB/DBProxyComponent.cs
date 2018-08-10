#if Server
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

        public async Task<T> QueryFirstOrDefault<T>(string sql)
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(dbAddress);
            var response = (DBQueryFirst.Response)await session.Call(new DBQueryFirst.Request() { collectionName = typeof(T).Name, sql = sql });
            return (T)response.data;
        }
    }
}
#endif
