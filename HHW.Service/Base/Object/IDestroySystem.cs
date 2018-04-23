using System;

namespace HHW.Service.Base.Object
{
    public interface IDestroySystem
    {
        Type Type();
        void Execute(object o);
    }

    public abstract class DestroySystem<T> : IDestroySystem
    {
        public void Execute(object o)
        {
            this.Destroy((T)o);
        }

        public Type Type()
        {
            return typeof(T);
        }
        public abstract void Destroy(T self);
    }
}
