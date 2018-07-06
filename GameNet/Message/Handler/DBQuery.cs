using System;
using System.Threading.Tasks;

namespace GN
{
    public class DBQuery : AMRpcHandler<DB.Query, DB.QueryResponse>
    {
        protected override Task<DB.QueryResponse> Run(Session session, DB.Query message)
        {
            throw new NotImplementedException();
        }
    }
}
