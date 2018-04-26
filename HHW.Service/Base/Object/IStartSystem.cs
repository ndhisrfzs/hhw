﻿using System;

namespace HHW.Service
{
    public interface IStartSystem : ISystem
    {
        void Execute(object o);
    }

    public abstract class StartSystem<T> : IStartSystem
    {
        public Type Type()
        {
            return typeof(T);
        }
        public void Execute(object o)
        {
            this.Start((T)o);
        }

        public abstract void Start(T self);
    }
}