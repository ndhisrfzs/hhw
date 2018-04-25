using System;
using System.Collections.Generic;

namespace HHW.Service
{
    public static class ComponentPool
    {
        private static readonly Dictionary<Type, Queue<Component>> dictionary = new Dictionary<Type, Queue<Component>>();

        public static Component Fetch(Type type)
        {
            if(!dictionary.TryGetValue(type, out Queue<Component> queue))
            {
                queue = new Queue<Component>();
                dictionary.Add(type, queue);
            }

            Component obj;
            if(queue.Count > 0)
            {
                obj = queue.Dequeue();
                obj.IsFromPool = true;
                return obj;
            }

            obj = (Component)Activator.CreateInstance(type);
            return obj;
        }

        public static void Recycle(Component obj)
        {
            Type type = obj.GetType();
            if(!dictionary.TryGetValue(type, out Queue<Component> queue))
            {
                queue = new Queue<Component>();
                dictionary.Add(type, queue);
            }
            queue.Enqueue(obj);
        }
    }
}
