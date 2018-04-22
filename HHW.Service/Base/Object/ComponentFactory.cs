using System;

namespace HHW.Service.Base.Object
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
    }
}
