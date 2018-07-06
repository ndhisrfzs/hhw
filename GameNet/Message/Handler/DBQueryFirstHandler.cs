using System;
using System.Threading.Tasks;

namespace GN
{
    public class DBQueryFirstHandler : AMRpcHandler<DB.QueryFirst, DB.QueryResponse>
    {
        protected override Task<DB.QueryResponse> Run(Session session, DB.QueryFirst message)
        {
            throw new NotImplementedException();
        }
    }
}
