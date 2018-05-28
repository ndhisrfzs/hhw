using System;
using System.Threading.Tasks;

namespace HHW.Service
{
    public abstract class AMRpcHandler<Request, Response> : IMHandler where Request : class, IRequest where Response : class, IResponse
    {
        protected static void ReplyError(Response response, Exception e, Action<Response> reply)
        {
            response.Error = ErrorCode.ERR_RpcFail;
            response.Message = e.ToString();
            reply(response);
        }
        protected abstract Task<Response> Run(Session session, Request message);//, Action<Response> reply);

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
                var response = await this.Run(session, request);
                //response =>
                //{
                if (session.IsDisposed)
                {
                    return;
                }
                response.RpcId = rpcId;
                session.Reply(response);
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public Type GetMessageType()
        {
            return typeof(Request);
        }
    }
}
