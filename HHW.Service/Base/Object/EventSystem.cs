using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static HHW.Service.FastInvoke;

namespace HHW.Service
{
    public enum DLLType
    {
        Model,
        Logic,
    }
    public struct InvokeInfo
    {
        public long id;
        public FastInvokeHandler invoke;

        public InvokeInfo(long id, FastInvokeHandler invoke)
        {
            this.id = id;
            this.invoke = invoke;
        }
    }
    public static class EventSystem
    {
        private static readonly Dictionary<long, Object> allComponents = new Dictionary<long, Object>();
        private static readonly Dictionary<DLLType, Assembly> assemblies = new Dictionary<DLLType, Assembly>();
        private static readonly List<Type> types = new List<Type>();

        private static readonly Queue<InvokeInfo> starts = new Queue<InvokeInfo>();

        private static Queue<InvokeInfo> loaders = new Queue<InvokeInfo>();
        private static Queue<InvokeInfo> tempLoaders = new Queue<InvokeInfo>();

        private static Queue<InvokeInfo> updates = new Queue<InvokeInfo>();
        private static Queue<InvokeInfo> tempUpdates = new Queue<InvokeInfo>();

        public static Assembly[] GetAll()
        {
            return assemblies.Values.ToArray();
        }

        public static void Add(DLLType dllType, Assembly assembly)
        {
            types.Clear();
            assemblies[dllType] = assembly;
            foreach (Assembly value in assemblies.Values)
            {
                types.AddRange(value.GetTypes());
            }
            //Load();
        }

        public static Assembly Get(DLLType dllType)
        {
            return assemblies[dllType];
        }

        public static List<Type> GetTypes()
        {
            return types;
        }

        public static void Add(Object obj)
        {
            allComponents.Add(obj.id, obj);

            Type type = obj.GetType();

            FastInvokeHandler loadInvoke = FastInvokeCache.GetMethod(type, "Load"); 
            if (loadInvoke != null)
            {
                loaders.Enqueue(new InvokeInfo(obj.id, loadInvoke));
            }
            FastInvokeHandler startInvoke = FastInvokeCache.GetMethod(type, "Start"); 
            if (startInvoke != null)
            {
                starts.Enqueue(new InvokeInfo(obj.id, startInvoke));
            }
            FastInvokeHandler updateInvoke = FastInvokeCache.GetMethod(type, "Update");
            if(updateInvoke != null)
            {
                updates.Enqueue(new InvokeInfo(obj.id, updateInvoke));
            }
        }

        public static void Load()
        {
            while(loaders.Count > 0)
            {
                InvokeInfo invokeInfo = loaders.Dequeue();
                Object obj;
                if (!allComponents.TryGetValue(invokeInfo.id, out obj))
                {
                    continue;
                }
                if (obj.IsDisposed)
                {
                    continue;
                }

                tempLoaders.Enqueue(invokeInfo);
                try
                {
                    invokeInfo.invoke(obj, null);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            SwapHelper.Swap(ref loaders, ref tempLoaders);
        }

        public static void Awake(Object obj)
        {
            Type type = obj.GetType();
            FastInvokeHandler awakeInvoke = FastInvokeCache.GetMethod(type, "Awake");
            if (awakeInvoke != null)
            {
                try
                {
                    awakeInvoke(obj, null);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public static void Awake<A>(Object obj, A a)
        {
            Type type = obj.GetType();
            FastInvokeHandler awakeInvoke = FastInvokeCache.GetMethod(type, "Awake", typeof(A));
            if (awakeInvoke != null)
            {
                try
                {
                    awakeInvoke(obj, new object[] { a });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public static void Awake<A, B>(Object obj, A a, B b)
        {
            Type type = obj.GetType();
            FastInvokeHandler awakeInvoke = FastInvokeCache.GetMethod(type, "Awake", typeof(A), typeof(B));
            if (awakeInvoke != null)
            {
                try
                {
                    awakeInvoke(obj, new object[] { a, b });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public static void Awake<A, B, C>(Object obj, A a, B b, C c)
        {
            Type type = obj.GetType();
            FastInvokeHandler awakeInvoke = FastInvokeCache.GetMethod(type, "Awake", typeof(A), typeof(B), typeof(C));
            if (awakeInvoke != null)
            {
                try
                {
                    awakeInvoke(obj, new object[] { a, b, c });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        private static void Start()
        {
            while(starts.Count > 0)
            {
                InvokeInfo invokeInfo = starts.Dequeue();
                Object obj;
                if (!allComponents.TryGetValue(invokeInfo.id, out obj))
                {
                    continue;
                }

                try
                {
                    invokeInfo.invoke(obj, null);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public static void Update()
        {
            Start();

            while(updates.Count > 0)
            {
                InvokeInfo invokeInfo = updates.Dequeue();
                Object obj;
                if (!allComponents.TryGetValue(invokeInfo.id, out obj))
                {
                    continue;
                }
                if (obj.IsDisposed)
                {
                    continue;
                }

                tempUpdates.Enqueue(invokeInfo);
                try
                {
                    invokeInfo.invoke(obj, null);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            SwapHelper.Swap(ref updates, ref tempUpdates);
        }

        public static void Destroy(Object obj)
        {
            Type type = obj.GetType();
            FastInvokeHandler destoryInvoke = FastInvokeCache.GetMethod(type, "Destroy");
            if (destoryInvoke != null)
            {
                try
                {
                    destoryInvoke(obj, null);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
