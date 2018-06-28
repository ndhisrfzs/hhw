using System;
using System.Threading.Tasks;

namespace HHW.Service
{
   
    public abstract class AMRpcHandler<Request, Response> : IMHandler where Request : class, IRequest where Response : class, IResponse
    {
        protected abstract Task<Response> Run(Session session, Request message);

        public async void Handle(Session session, object message)
        {
            try
            {
                Request request = message as Request;
                if (request == null)
                {
                    return;
                }

                uint rpcId = request.RpcId;
                long sessionid = session.id;
                var response = await this.Run(session, request);
                if (session.id != sessionid)
                {
                    return;
                }
                response.RpcId = rpcId;
                session.Reply(response);
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
