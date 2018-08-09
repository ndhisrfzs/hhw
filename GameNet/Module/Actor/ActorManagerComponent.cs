using System;
using System.Collections.Generic;
using System.Text;

namespace GN
{
    public class ActorManagerComponent : Component
    {
        private Dictionary<long, Entity> Actors = new Dictionary<long, Entity>();
        public bool Add(Entity entity)
        {
            return Actors.TryAdd(entity.id, entity);
        }

        public bool Remove(long id)
        {
            return Actors.Remove(id);
        }

        public Entity GetActor(long id)
        {
            Entity entity;
            if(Actors.TryGetValue(id, out entity))
            {
                return entity;
            }
            return null;
        }
    }
}
