using System;

namespace HHW.Service
{
    public interface ILoadSystem : ISystem
    {
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
