using System;
using System.Threading.Tasks;

namespace GN
{
   
    public abstract class AMRpcHandler<Request, Response> : IMHandler where Request : class, IRequest where Response : class, IResponse
    {
        protected abstract Task<Response> Run(Session session, Request message);

        public async void Handle(Session session, uint rpcId, object message)
        {
            try
            {
                Request request = message as Request;
                if (request == null)
                {
                    return;
                }

                long sessionid = session.id;
                var response = await this.Run(session, request);
                if (session.id != sessionid)
                {
                    return;
                }
                session.Reply(rpcId, response);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
        public Type GetMessageType()
        {
            return typeof(Request);
        }
    }
}
