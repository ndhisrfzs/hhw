using System;

namespace HHW.Service
{
    public static class ObjectFactory
    {
        public static Component CreateWithParent(Type type, Entity parent)
        {
            Component component = ObjectPool.Fetch<Component>();
            component.Parent = parent;
            EventSystem.Awake(component);
            return component;
        }

        public static T CreateWithParent<T>(Object parent) where T : Object
        {
            T obj = ObjectPool.Fetch<T>();
            obj.Parent = parent;
            EventSystem.Awake(obj);
            return obj;
        }
        public static T CreateWithParent<T, A>(Object parent, A a) where T : Object
        {
            T obj = ObjectPool.Fetch<T>();
            obj.Parent = parent;
            EventSystem.Awake(obj, a);
            return obj;
        }

        public static T CreateWithParent<T, A, B>(Object parent, A a, B b) where T : Object
        {
            T obj = ObjectPool.Fetch<T>();
            obj.Parent = parent;
            EventSystem.Awake(obj, a, b);
            return obj;
        }
        public static T CreateWithParent<T, A, B, C>(Object parent, A a, B b, C c) where T : Object
        {
            T obj = ObjectPool.Fetch<T>();
            obj.Parent = parent;
            EventSystem.Awake(obj, a, b, c);
            return obj;
        }

        public static T Create<T, A, B>(A a, B b) where T : Object
        {
            T obj = ObjectPool.Fetch<T>();
            EventSystem.Awake(obj, a, b);
            return obj;
        }
    }
}
