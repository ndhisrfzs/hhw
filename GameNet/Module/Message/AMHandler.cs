using System;
using System.Threading.Tasks;

namespace GN
{
    public abstract class AMHandler<E, Request> : IMHandler where E : Entity where Request : class, IRequest
    {
        protected abstract Task Run(E entity, Request message);
        public async Task Handle(Session session, Entity entity, uint rpcId, object message)
        {
            try
            {
                Request request = message as Request;
                if (request == null)
                {
                    return;
                }

                await this.Run(entity as E, request);
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
