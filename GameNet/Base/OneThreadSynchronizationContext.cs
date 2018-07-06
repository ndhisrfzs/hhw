using System;
using System.Collections.Concurrent;
using System.Threading;

namespace GN
{
    public class OneThreadSynchronizationContext : SynchronizationContext
    {
        private static OneThreadSynchronizationContext m_Instance = null;
        public static OneThreadSynchronizationContext Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new OneThreadSynchronizationContext();
                }
                return m_Instance;
            }
        }


        // 线程同步队列,发送接收socket回调都放到该队列,由poll线程统一执行
        private readonly ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();

        private Action a;

        private void Add(Action action)
        {
            this.queue.Enqueue(action);
        }

        public void Update()
        {
            while (true)
            {
                if (!this.queue.TryDequeue(out a))
                {
                    return;
                }
                a();
            }
        }

        public override void Post(SendOrPostCallback callback, object state)
        {
#if Server
            callback(state);
#else
            this.Add(() => { callback(state); });
#endif
        }
    }
}
