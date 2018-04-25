using System;

namespace HHW.Service
{
    public abstract class Object : IDisposable
    {
        public long Id { get; protected set; }
        protected Object()
        {
            this.Id = IdGenerater.GenerateId();
        }
        protected Object(long id)
        {
            this.Id = id;
        }

        public bool IsDisposed
        {
            get
            {
                return this.Id == 0;
            }
        }

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
