using System;

namespace HHW.Service.Base.Object
{
    public interface IAwakeSystem
    {
        Type Type();
    }
    public interface IAwake : IAwakeSystem
    {
        void Execute(object o);
    }

    public interface IAwake<A> : IAwakeSystem
    {
        void Execute(object o, A a);
    }

    public interface IAwake<A, B> : IAwakeSystem
    {
        void Execute(object o, A a, B b);
    }

    public interface IAwake<A, B, C> : IAwakeSystem
    {
        void Execute(object o, A a, B b, C c);
    }

    public abstract class AwakeSystem<T> : IAwake
    {
        public Type Type()
        {
            return typeof(T);
        }
        public void Execute(object o)
        {
            this.Awake((T)o);
        }

        public abstract void Awake(T self);
    }

    public abstract class AwakeSystem<T, A> : IAwake<A>
    {
        public Type Type()
        {
            return typeof(T);
        }
        public void Execute(object o, A a)
        {
            this.Awake((T)o, a);
        }

        public abstract void Awake(T self, A a);
    }

    public abstract class AwakeSystem<T, A, B> : IAwake<A, B>
    {
        public Type Type()
        {
            return typeof(T);
        }
        public void Execute(object o, A a, B b)
        {
            this.Awake((T)o, a, b);
        }

        public abstract void Awake(T self, A a, B b);
    }

    public abstract class AwakeSystem<T, A, B, C> : IAwake<A, B, C>
    {
        public Type Type()
        {
            return typeof(T);
        }
        public void Execute(object o, A a, B b, C c)
        {
            this.Awake((T)o, a, b, c);
        }
        public abstract void Awake(T self, A a, B b, C c);
    }
}
