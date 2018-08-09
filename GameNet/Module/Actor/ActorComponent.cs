using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GN
{
    public struct ActorMessage
    {
        public Session session { get; set; }
        public ushort opcode { get; set; }
        public uint rpcId { get; set; }
        public object message { get; set; }
    }
    public class ActorComponent : Component
    {
        private IActorHandler ActorHandler;
        private Queue<ActorMessage> ActorMessages = new Queue<ActorMessage>();
        private TaskCompletionSource<ActorMessage> Tcs;
        public void Awake()
        {
            ActorHandler = new CommonActorHandler();
            ActorMessages.Clear();
            AddLocaltion();
        }

        public void Awake(ActorHandlerType handlerType)
        {
            switch(handlerType)
            {
                case ActorHandlerType.Gate:
                    ActorHandler = new GateActorHandler();
                    break;
                case ActorHandlerType.Common:
                    ActorHandler = new CommonActorHandler();
                    break;
            }
            ActorMessages.Clear();
            AddLocaltion();
        }

        public void Destroy()
        {
            RemoveLocaltion();
        }

        public async void Start()
        {
            while(true)
            {
                try
                {
                    ActorMessage msg = await GetAsync();

                    if(msg.message == null)
                    {
                        return;
                    }

                    await ActorHandler.Handle(msg.session, new MessageInfo(msg.opcode, msg.rpcId, (this.Parent as Entity), msg.message));
                }
                catch(Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void AddLocaltion()
        {
            Game.Scene.GetComponent<ActorManagerComponent>().Add(this.Parent as Entity);
        }

        public void RemoveLocaltion()
        {
            Game.Scene.GetComponent<ActorManagerComponent>().Remove(Parent.id);
        }

        public void Add(ActorMessage message)
        {
            this.ActorMessages.Enqueue(message);
            if(this.Tcs == null)
            {
                return;
            }

            var t = this.Tcs;
            this.Tcs = null;
            t.SetResult(this.ActorMessages.Dequeue());
        }

        private Task<ActorMessage> GetAsync()
        {
            if(this.ActorMessages.Count > 0)
            {
                return Task.FromResult(this.ActorMessages.Dequeue());
            }

            this.Tcs = new TaskCompletionSource<ActorMessage>();
            return this.Tcs.Task;
        }

        public override void Dispose()
        {
            if(this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            var t = this.Tcs;
            this.Tcs = null;
            t?.SetResult(new ActorMessage());
        }
    }
}
