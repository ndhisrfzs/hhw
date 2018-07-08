using System;

namespace GN
{
    public abstract class AMHandler<Request> : IMHandler where Request : class, IRequest
    {
        protected abstract void Run(Session session, Request message);
        public void Handle(Session session, object message)
        {
            try
            {
                Request request = message as Request;
                if (request == null)
                {
                    return;
                }

                this.Run(session, request);
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
