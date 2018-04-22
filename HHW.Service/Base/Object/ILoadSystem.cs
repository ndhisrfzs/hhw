using System;

namespace HHW.Service.Base.Object
{
    public interface ILoadSystem
    {
        Type Type();
        void Execute(object o);
    }

    public abstract class LoadSystem<T> : ILoadSystem
    {
        public void Execute(object o)
        {
            this.Load((T)o);
        }

        public Type Type()
        {
            return typeof(T);
        }

        public abstract void Load(T self);
    }
}
