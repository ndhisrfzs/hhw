using System;
using System.Collections.Generic;

namespace HHW.Service
{
    public static class ObjectPool
    {
        private static readonly Dictionary<Type, Queue<Object>> dictionary = new Dictionary<Type, Queue<Object>>();

        public static Object Fetch(Type type)
        {
            if(!dictionary.TryGetValue(type, out Queue<Object> queue))
            {
                queue = new Queue<Object>();
                dictionary.Add(type, queue);
            }

            Object obj;
            if(queue.Count > 0)
            {
                obj = queue.Dequeue();
                obj.IsFromPool = true;
                return obj;
            }

            obj = (Object)Activator.CreateInstance(type);
            obj.IsFromPool = true;
            return obj;
        }

        public static void Recycle(Object obj)
        {
            Type type = obj.GetType();
            if(!dictionary.TryGetValue(type, out Queue<Object> queue))
            {
                queue = new Queue<Object>();
                dictionary.Add(type, queue);
            }
            queue.Enqueue(obj);
        }
    }
}
