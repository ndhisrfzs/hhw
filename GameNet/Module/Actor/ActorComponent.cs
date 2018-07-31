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
        private Queue<ActorMessage> m_Queue = new Queue<ActorMessage>();
        private TaskCompletionSource<ActorMessage> Tcs;
        public void Awake()
        {
            m_Queue.Clear();
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

                    await Game.Scene.GetComponent<MessageDispatherComponent>().Handle(msg.session, new MessageInfo(msg.opcode, msg.rpcId, Parent as Entity, msg.message));
                }
                catch(Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public async Task AddLocaltion()
        {
            //await Game.Scene.GetComponent<LocationProxyComponent>().Add(Parent.id);
        }

        public async Task RemoveLocaltion()
        {
            //await Game.Scene.GetComponent<LocationProxyComponent>().Remove(Parent.id);
        }

        public void Add(ActorMessage message)
        {
            this.m_Queue.Enqueue(message);
            if(this.Tcs == null)
            {
                return;
            }

            var t = this.Tcs;
            this.Tcs = null;
            t.SetResult(this.m_Queue.Dequeue());
        }

        private Task<ActorMessage> GetAsync()
        {
            if(this.m_Queue.Count > 0)
            {
                return Task.FromResult(this.m_Queue.Dequeue());
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
