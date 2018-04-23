using System;

namespace HHW.Service.Base.Object
{
    public interface IUpdateSystem
    {
        Type Type();
        void Execute(object o);
    }

    public abstract class UpdateSystem<T> : IUpdateSystem
    {
        public void Execute(object o)
        {
            this.Update((T)o);
        }

        public Type Type()
        {
            return typeof(T);
        }

        public abstract void Update(T self);
    }
}
