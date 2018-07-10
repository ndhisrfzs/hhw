using System;
using System.Threading.Tasks;

namespace GN
{
    [MessageHandler(AppType.DB)]
    public class DBQueryHandler : AMRpcHandler<DBQuery.Request, DBQuery.Response>
    {
        protected override Task<DBQuery.Response> Run(Session session, DBQuery.Request message)
        {
            throw new NotImplementedException();
        }
    }
}
