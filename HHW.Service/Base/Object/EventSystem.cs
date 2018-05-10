using System;
using System.Collections.Generic;
using System.Reflection;
using static HHW.Service.FastInvoke;

namespace HHW.Service
{
    public enum DLLType
    {
        Model,
        Hotfix,
        Editor,
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
        //private static readonly Dictionary<DLLType, Assembly> assemblies = new Dictionary<DLLType, Assembly>();

        private static readonly Queue<InvokeInfo> starts = new Queue<InvokeInfo>();

        private static Queue<InvokeInfo> loaders = new Queue<InvokeInfo>();
        private static Queue<InvokeInfo> tempLoaders = new Queue<InvokeInfo>();

        private static Queue<InvokeInfo> updates = new Queue<InvokeInfo>();
        private static Queue<InvokeInfo> tempUpdates = new Queue<InvokeInfo>();

        //public static Assembly[] GetAll()
        //{
        //    return assemblies.Values.ToArray();
        //}

        //public static void Add(DLLType dllType, Assembly assembly)
        //{
        //    assemblies[dllType] = assembly;
        //    Load();
        //}

        public static void Add(Object obj)
        {
            allComponents.Add(obj.id, obj);

            Type type = obj.GetType();

            MethodInfo loadInfo = type.GetMethod("Load", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { }, null);
            if(loadInfo != null)
            {
                loaders.Enqueue(new InvokeInfo(obj.id, GetMethodInvoker(loadInfo)));
            }
            MethodInfo startInfo = type.GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { }, null);
            if(startInfo != null)
            {
                starts.Enqueue(new InvokeInfo(obj.id, GetMethodInvoker(startInfo)));
            }
            MethodInfo updateInfo = type.GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { }, null);
            if(updateInfo != null)
            {
                updates.Enqueue(new InvokeInfo(obj.id, GetMethodInvoker(updateInfo)));
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
            MethodInfo awakeInfo = type.GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { }, null);
            if (awakeInfo != null)
            {
                try
                {
                    awakeInfo.Invoke(obj, null);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public static void Awake<A>(Object obj, A a)
        {
            Type type = obj.GetType();
            MethodInfo awakeInfo = type.GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(A) }, null);
            if (awakeInfo != null)
            {
                try
                {
                    awakeInfo.Invoke(obj, new object[] { a });
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
            MethodInfo awakeInfo = type.GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(A), typeof(B) }, null);
            if (awakeInfo != null)
            {
                try
                {
                    awakeInfo.Invoke(obj, new object[] { a, b });
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
            MethodInfo awakeInfo = type.GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(A), typeof(B), typeof(C) }, null);
            if (awakeInfo != null)
            {
                try
                {
                    awakeInfo.Invoke(obj, new object[] { a, b, c });
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
                    Console.WriteLine(e.ToString());
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
            MethodInfo destroyInfo = type.GetMethod("Destroy", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { }, null);
            if (destroyInfo != null)
            {
                try
                {
                    destroyInfo.Invoke(obj, null);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
