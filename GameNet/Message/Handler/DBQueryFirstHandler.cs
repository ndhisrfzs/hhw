using System;
using System.Threading.Tasks;

namespace GN
{
    public class DBQueryFirstHandler : AMRpcHandler<DBQueryFirst.Request, DBQueryFirst.Response>
    {
        protected override Task<DBQueryFirst.Response> Run(Session session, DBQueryFirst.Request message)
        {
            throw new NotImplementedException();
        }
    }
}
