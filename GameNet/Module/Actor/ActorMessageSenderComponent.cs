using System;
using System.Collections.Generic;
using System.Text;

namespace GN
{
    public class ActorMessageSenderComponent : Component
    {
        private readonly Dictionary<long, ActorMessageSender> ActorMessageSenders = new Dictionary<long, ActorMessageSender>();
        public override void Dispose()
        {
            if(this.IsDisposed)
            {
                return;
            }
            base.Dispose();

            foreach(ActorMessageSender actorMessageSender in this.ActorMessageSenders.Values)
            {
                actorMessageSender.Dispose();
            }

            this.ActorMessageSenders.Clear();
        }

        public ActorMessageSender Get(long actorId)
        {
            ActorMessageSender actorMessageSender;
            if (this.ActorMessageSenders.TryGetValue(actorId, out actorMessageSender))
            {
                return actorMessageSender;
            }

            actorMessageSender = ObjectFactory.CreateWithParent<ActorMessageSender, long>(this, actorId);
            this.ActorMessageSenders[actorId] = actorMessageSender;
            return actorMessageSender;
        }

        public void Remove(long actorId)
        {
            ActorMessageSender actorMessageSender;
            if(!this.ActorMessageSenders.TryGetValue(actorId, out actorMessageSender))
            {
                return;
            }

            this.ActorMessageSenders.Remove(actorId);
            actorMessageSender.Dispose();
        }
    }
}
