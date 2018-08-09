using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace GN
{
    public struct ActorTask
    {
        public IRequest message;
        public TaskCompletionSource<IResponse> tcs;

        public ActorTask(IRequest message)
        {
            this.message = message;
            this.tcs = null;
        }

        public ActorTask(IRequest message, TaskCompletionSource<IResponse> tcs)
        {
            this.message = message;
            this.tcs = tcs;
        }
    }

    public class ActorMessageSender : Entity
    {
        private IPEndPoint Address;
        private long ActorId;
        private Queue<ActorTask> WaitingMessages = new Queue<ActorTask>();
        private TaskCompletionSource<ActorTask> Tcs;
        private int Error;

        public async void Awake()
        {
            this.ActorId = this.id;
            int appId = IdGenerater.GetAppIdFromId(ActorId);
            AppInfo appInfo = await Game.Scene.GetComponent<SlaveComponent>().Get(appId);
            this.Address = appInfo.innerAddress.IpEndPoint();
        }

        public void Destroy()
        {
            ActorTask actorTask;
            while (WaitingMessages.TryDequeue(out actorTask))
            {
                actorTask.tcs?.SetException(new RpcException(Error, ""));
            }

            var t = Tcs;
            Tcs = null;
            t?.SetResult(new ActorTask());
        }

        private void Add(ActorTask task)
        {
            if(IsDisposed)
            {
                throw new Exception("GateSessionActorId Disposed! dont hold actorproxy");
            }

            WaitingMessages.Enqueue(task);

            AllowGet();
        }

        private void AllowGet()
        {
            if(this.Tcs == null || this.WaitingMessages.Count <= 0)
            {
                return;
            }

            ActorTask task = WaitingMessages.Peek();
            var t = this.Tcs;
            this.Tcs = null;
            t?.SetResult(task);
        }

        private Task<ActorTask> GetAsync()
        {
            ActorTask task;
            if(WaitingMessages.TryPeek(out task))
            {
                return Task.FromResult(task);
            }

            this.Tcs = new TaskCompletionSource<ActorTask>();
            return this.Tcs.Task;
        }

        public async void UpdateAsync()
        {
            try
            {
                while(true)
                {
                    ActorTask task = await GetAsync();
                    if(this.IsDisposed)
                    {
                        return;
                    }

                    await RunTask(task);
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }
        }

        private async Task RunTask(ActorTask task)
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(this.Address);
            task.message.ActorId = this.ActorId;
            IResponse response = await session.Call(task.message);
            switch(response.Error)
            {
                case ErrorCode.ERR_Success:
                    {
                        this.WaitingMessages.Dequeue();
                        task.tcs?.SetResult(response);
                    }
                    break;
                default:
                    {
                        this.Error = response.Error;
                        Game.Scene.GetComponent<ActorMessageSenderComponent>().Remove(ActorId);
                    }
                    break;
            }
        }

        public Task<IResponse> Call(IRequest request)
        {
            TaskCompletionSource<IResponse> tcs = new TaskCompletionSource<IResponse>();
            ActorTask task = new ActorTask(request, tcs);
            this.Add(task);
            return task.tcs.Task;
        }

        public void Send(IRequest request)
        {
            ActorTask task = new ActorTask(request);
            this.Add(task);
        }

        public override void Dispose()
        {
            if(IsDisposed)
            {
                return;
            }
            base.Dispose();
        }
    }
}
