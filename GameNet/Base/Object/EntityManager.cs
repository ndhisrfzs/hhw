using System.Collections.Generic;

namespace GN
{
    public class EntityManager
    {
        private readonly Dictionary<long, Entity> allEntities = new Dictionary<long, Entity>();

        public void Add(Entity entity)
        {
            allEntities.TryAdd(entity.id, entity);
        }

        public Entity Get(long id)
        {
            Entity entity;
            allEntities.TryGetValue(id, out entity);
            return entity;
        }

        public void Remove(long id)
        {
            allEntities.Remove(id); 
        }
    }
}
