using System;

namespace HHW.Service
{
    public abstract class Component : IDisposable
    {
        public long InstanceId { get; protected set; }
        protected Component()
        {
            this.InstanceId = IdGenerater.GenerateId();
            EventSystem.Add(this);
        }

        private bool isFromPool;
        public bool IsFromPool
        {
            get
            {
                return isFromPool;
            }
            set
            {
                isFromPool = value;
                if(InstanceId == 0)
                {
                    InstanceId = IdGenerater.GenerateId();
                    EventSystem.Add(this);
                }
            }
        }
        public bool IsDisposed
        {
            get
            {
                return InstanceId == 0;
            }
        }
        public Entity Parent { get; set; }

        public virtual void Dispose()
        {
            if(this.IsDisposed)
            {
                return;
            }

            this.InstanceId = 0;
            if(this.isFromPool)
            {
                ComponentPool.Recycle(this);
            }

            EventSystem.Destroy(this);
        }
    }
}
