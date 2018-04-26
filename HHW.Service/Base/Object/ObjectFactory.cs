using System;

namespace HHW.Service
{
    public static class ObjectFactory
    {
        public static Component CreateWithParent(Type type, Entity parent)
        {
            Component component = (Component)ObjectPool.Fetch(type);
            component.Parent = parent;
            EventSystem.Awake(component);
            return component;
        }

        public static T CreateWithParent<T>(Entity parent) where T : Object
        {
            T obj = (T)ObjectPool.Fetch(typeof(T));
            obj.Parent = parent;
            EventSystem.Awake(obj);
            return obj;
        }
        public static T CreateWithParent<T, A>(Entity parent, A a) where T : Object
        {
            T obj = (T)ObjectPool.Fetch(typeof(T));
            obj.Parent = parent;
            EventSystem.Awake(obj, a);
            return obj;
        }

        public static T CreateWithParent<T, A, B>(Entity parent, A a, B b) where T : Object
        {
            T obj = (T)ObjectPool.Fetch(typeof(T));
            obj.Parent = parent;
            EventSystem.Awake(obj, a, b);
            return obj;
        }
        public static T CreateWithParent<T, A, B, C>(Entity parent, A a, B b, C c) where T : Object
        {
            T obj = (T)ObjectPool.Fetch(typeof(T));
            obj.Parent = parent;
            EventSystem.Awake(obj, a, b, c);
            return obj;
        }

        public static T Create<T, A, B>(A a, B b) where T : Object
        {
            T obj = (T)ObjectPool.Fetch(typeof(T));
            EventSystem.Awake(obj, a, b);
            return obj;
        }
    }
}
