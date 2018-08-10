using System;

namespace GN
{
    public static class ObjectFactory
    {
        public static Component CreateWithParent(Type type, Entity parent)
        {
            Component component = ObjectPool.Fetch<Component>();
            component.Parent = parent;
            Game.EventSystem.Awake(component);
            return component;
        }

        public static T CreateWithParent<T>(Object parent) where T : Object
        {
            T obj = ObjectPool.Fetch<T>();
            obj.Parent = parent;
            Game.EventSystem.Awake(obj);
            return obj;
        }
        public static T CreateWithParent<T, A>(Object parent, A a) where T : Object
        {
            T obj = ObjectPool.Fetch<T>();
            obj.Parent = parent;
            Game.EventSystem.Awake(obj, a);
            return obj;
        }

        public static T CreateWithParent<T, A, B>(Object parent, A a, B b) where T : Object
        {
            T obj = ObjectPool.Fetch<T>();
            obj.Parent = parent;
            Game.EventSystem.Awake(obj, a, b);
            return obj;
        }
        public static T CreateWithParent<T, A, B, C>(Object parent, A a, B b, C c) where T : Object
        {
            T obj = ObjectPool.Fetch<T>();
            obj.Parent = parent;
            Game.EventSystem.Awake(obj, a, b, c);
            return obj;
        }

        public static T CreateWithId<T>(long id) where T : Object
        {
            T obj = ObjectPool.Fetch<T>(id);
            Game.EventSystem.Awake(obj);
            return obj;
        }

        public static T Create<T, A, B>(A a, B b) where T : Object
        {
            T obj = ObjectPool.Fetch<T>();
            Game.EventSystem.Awake(obj, a, b);
            return obj;
        }

        public static T Create<T>() where T :Object
        {
            T obj = ObjectPool.Fetch<T>();
            Game.EventSystem.Awake(obj);
            return obj;
        }

        public static T Create<T, A>(A a) where T : Object
        {
            T obj = ObjectPool.Fetch<T>();
            Game.EventSystem.Awake(obj, a);
            return obj;
        }
    }
}
