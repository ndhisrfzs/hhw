using System;
using System.Threading.Tasks;

namespace GN
{
   
    public abstract class AMRpcHandler<E, Request, Response> : IMHandler where E : Entity where Request : class, IRequest where Response : class, IResponse
    {
        protected abstract Task<Response> Run(E entity, Request message);

        public async Task Handle(Session session, Entity entity, uint rpcId, object message)
        {
            try
            {
                Request request = message as Request;
                if (request == null)
                {
                    return;
                }

                long sessionid = session.id;
                var response = await this.Run(entity as E, request);
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
