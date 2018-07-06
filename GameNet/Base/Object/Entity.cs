using System;
using System.Collections.Generic;
using System.Linq;

namespace GN
{
    public class Entity : Object
    {
        private Dictionary<Type, Component> componentDict;

        protected Entity()
        {
            this.componentDict = new Dictionary<Type, Component>();
        }

        protected Entity(long id)
            : base(id)
        {
            this.componentDict = new Dictionary<Type, Component>();
        }

        public override void Dispose()
        {
            if(this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            foreach (Component component in this.GetComponents())
            {
                try
                {
                    component.Dispose();
                }
                catch (Exception)
                {
                    throw;
                }
            }

            this.componentDict.Clear();
        }

        public Component AddComponent(Type type)
        {
            if(this.componentDict.ContainsKey(type))
            {
                throw new Exception($"AddComponent, Component already exist, id:{this.id}, Component:{type.Name}");
            }

            Component component = ObjectFactory.CreateWithParent(type, this);
            this.componentDict.Add(component.GetType(), component);
            return component;
        }

        public T AddComponent<T>() where T : Component, new()
        {
            if (this.componentDict.ContainsKey(typeof(T)))
            {
                throw new Exception($"AddComponent, Component already exist, id:{this.id}, Component:{typeof(T).Name}");
            }

            T component = ObjectFactory.CreateWithParent<T>(this);
            this.componentDict.Add(component.GetType(), component);
            return component;
        }

        public T AddComponent<T, A>(A a) where T : Component, new()
        {
            if (this.componentDict.ContainsKey(typeof(T)))
            {
                throw new Exception($"AddComponent, Component already exist, id:{this.id}, Component:{typeof(T).Name}");
            }

            T component = ObjectFactory.CreateWithParent<T, A>(this, a);
            this.componentDict.Add(component.GetType(), component);
            return component;
        }

        public T AddComponent<T, A, B>(A a, B b) where T : Component, new()
        {
            if (this.componentDict.ContainsKey(typeof(T)))
            {
                throw new Exception($"AddComponent, Component already exist, id:{this.id}, Component:{typeof(T).Name}");
            }

            T component = ObjectFactory.CreateWithParent<T, A, B>(this, a, b);
            this.componentDict.Add(component.GetType(), component);
            return component;
        }

        public T AddComponent<T, A, B, C>(A a, B b, C c) where T : Component, new()
        {
            if (this.componentDict.ContainsKey(typeof(T)))
            {
                throw new Exception($"AddComponent, Component already exist, id:{this.id}, Component:{typeof(T).Name}");
            }

            T component = ObjectFactory.CreateWithParent<T, A, B, C>(this, a, b, c);
            this.componentDict.Add(component.GetType(), component);
            return component;
        }

        public void RemoveComponent<T>() where T : Component
        {
            RemoveComponent(typeof(T));
        }

        public void RemoveComponent(Type type)
        {
            Component component;
            if (this.componentDict.TryGetValue(type, out component))
            {
                if (this.componentDict.Remove(type))
                {
                    component.Dispose();
                }
            }
        }

        public T GetComponent<T>() where T : Component
        {
            Component component;
            if(!this.componentDict.TryGetValue(typeof(T), out component))
            {
                return default(T);
            }
            return (T)component;
        }

        public Component GetComponent(Type type)
        {
            Component component;
            if(!this.componentDict.TryGetValue(type, out component))
            {
                return null;
            }

            return component;
        }

        public Component[] GetComponents()
        {
            return this.componentDict.Values.ToArray();
        }
    }
}
