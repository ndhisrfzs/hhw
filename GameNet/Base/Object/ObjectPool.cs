using System;
using System.Collections.Generic;

namespace GN
{
    public static class ObjectPool
    {
        private static readonly Dictionary<Type, Queue<Object>> dictionary = new Dictionary<Type, Queue<Object>>();

        public static T Fetch<T>(long id)
            where T : Object
        {
            T t = fetch<T>();
            t.id = id;
            t.AddManager();
            return t;
        }

        public static T Fetch<T>()
            where T : Object
        {
            T t = fetch<T>();
            t.AddManager();
            return t;
        }

        private static T fetch<T>()
            where T : Object
        {
            Type type = typeof(T);
            Queue<Object> queue = null;
            if(!dictionary.TryGetValue(type, out queue))
            {
                queue = new Queue<Object>();
                dictionary.Add(type, queue);
            }

            T obj;
            if(queue.Count > 0)
            {
                obj = (T)queue.Dequeue();
                obj.IsFromPool = true;
                return obj;
            }

            obj = (T)Activator.CreateInstance(type);
            obj.IsFromPool = true;
            return obj;
        }

        public static void Recycle(Object obj)
        {
            Type type = obj.GetType();
            Queue<Object> queue = null;
            if(!dictionary.TryGetValue(type, out queue))
            {
                queue = new Queue<Object>();
                dictionary.Add(type, queue);
            }
            queue.Enqueue(obj);
        }
    }
}
