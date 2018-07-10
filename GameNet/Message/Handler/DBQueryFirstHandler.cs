using Dapper;
using System.Threading.Tasks;

namespace GN
{
    [MessageHandler(AppType.DB)]
    public class DBQueryFirstHandler : AMRpcHandler<DBQueryFirst.Request, DBQueryFirst.Response>
    {
        protected override Task<DBQueryFirst.Response> Run(Session session, DBQueryFirst.Request message)
        {
            DBQueryFirst.Response resp = new DBQueryFirst.Response();
            var type = Game.Scene.GetComponent<DBModelTypeComponent>().GetType(message.collectionName);
            var conn = Game.Scene.GetComponent<DBComponent>().dbConnection;
            resp.data = conn.QueryFirstOrDefault(type, message.sql, message.parms);
            return Task.FromResult(resp);
        }
    }
}
