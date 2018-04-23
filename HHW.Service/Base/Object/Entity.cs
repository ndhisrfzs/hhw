using HHW.Service.Base.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HHW.Service.Base.Object
{
    public class Entity : Object
    {
        private Dictionary<Type, Component> componentDict;

        protected Entity()
        {
            this.componentDict = new Dictionary<Type, Component>();
        }

        protected Entity(long id): base(id)
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
                throw new Exception($"AddComponent, Component already exist, id:{this.Id}, Component:{type.Name}");
            }

            Component component = ComponentFactory.CreateWithParent(type, this);
            this.componentDict.Add(component.GetType(), component);
            return component;
        }

        public Component[] GetComponents()
        {
            return this.componentDict.Values.ToArray();
        }
    }
}
