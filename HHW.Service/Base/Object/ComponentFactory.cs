using System;

namespace HHW.Service
{
    public static class ComponentFactory
    {
        public static Component CreateWithParent(Type type, Entity parent)
        {
            Component component = ComponentPool.Fetch(type);
            component.Parent = parent;
            EventSystem.Awake(component);
            return component;
        }

        public static T CreateWithParent<T>(Entity parent) where T : Component
        {
            T component = (T)ComponentPool.Fetch(typeof(T));
            component.Parent = parent;
            EventSystem.Awake(component);
            return component;
        }
        public static T CreateWithParent<T, A>(Entity parent, A a) where T : Component
        {
            T component = (T)ComponentPool.Fetch(typeof(T));
            component.Parent = parent;
            EventSystem.Awake(component, a);
            return component;
        }

        public static T CreateWithParent<T, A, B>(Entity parent, A a, B b) where T : Component
        {
            T component = (T)ComponentPool.Fetch(typeof(T));
            component.Parent = parent;
            EventSystem.Awake(component, a, b);
            return component;
        }
        public static T CreateWithParent<T, A, B, C>(Entity parent, A a, B b, C c) where T : Component
        {
            T component = (T)ComponentPool.Fetch(typeof(T));
            component.Parent = parent;
            EventSystem.Awake(component, a, b, c);
            return component;
        }
    }
}
