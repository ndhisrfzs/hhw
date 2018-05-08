﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        //private static readonly UnOrderMultiMap<Type, IAwakeSystem> awakeSystems = new UnOrderMultiMap<Type, IAwakeSystem>();
        //private static readonly UnOrderMultiMap<Type, IStartSystem> startSystems = new UnOrderMultiMap<Type, IStartSystem>();
        //private static readonly UnOrderMultiMap<Type, ILoadSystem> loadSystems = new UnOrderMultiMap<Type, ILoadSystem>();
        //private static readonly UnOrderMultiMap<Type, IUpdateSystem> updateSystems = new UnOrderMultiMap<Type, IUpdateSystem>();
        //private static readonly UnOrderMultiMap<Type, IDestroySystem> destroySystems = new UnOrderMultiMap<Type, IDestroySystem>();

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

        //    awakeSystems.Clear();
        //    startSystems.Clear();
        //    loadSystems.Clear();
        //    updateSystems.Clear();
        //    destroySystems.Clear();

        //    Type[] types = DllHelper.GetMonoTypes(GetAll());
        //    foreach (Type type in types)
        //    {
        //        if (type.IsInterface || type.IsAbstract || !typeof(ISystem).IsAssignableFrom(type))
        //        {
        //            continue;
        //        }

        //        object obj = Activator.CreateInstance(type);

        //        ILoadSystem loadSystem = obj as ILoadSystem;
        //        if(loadSystem != null)
        //        {
        //            loadSystems.Add(loadSystem.Type(), loadSystem);
        //        }

        //        IAwakeSystem awakeSystem = obj as IAwakeSystem;
        //        if(awakeSystem != null)
        //        {
        //            awakeSystems.Add(awakeSystem.Type(), awakeSystem);
        //        }

        //        IStartSystem startSystem = obj as IStartSystem;
        //        if(startSystem != null)
        //        {
        //            startSystems.Add(startSystem.Type(), startSystem);
        //        }

        //        IUpdateSystem updateSystem = obj as IUpdateSystem;
        //        if(updateSystem != null)
        //        {
        //            updateSystems.Add(updateSystem.Type(), updateSystem);
        //        }

        //        IDestroySystem destroySystem = obj as IDestroySystem;
        //        if(destroySystem != null)
        //        {
        //            destroySystems.Add(destroySystem.Type(), destroySystem);
        //        }
        //    }

        //    Load();
        //}

        public static void Add(Object obj)
        {
            allComponents.Add(obj.id, obj);

            Type type = obj.GetType();

            MethodInfo loadInfo = type.GetMethod("Load");
            if(loadInfo != null)
            {
                loaders.Enqueue(new InvokeInfo(obj.id, GetMethodInvoker(loadInfo)));
            }
            MethodInfo startInfo = type.GetMethod("Start");
            if(startInfo != null)
            {
                starts.Enqueue(new InvokeInfo(obj.id, GetMethodInvoker(startInfo)));
            }
            MethodInfo updateInfo = type.GetMethod("Update");
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
            MethodInfo awakeInfo = type.GetMethod("Awake");
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
            MethodInfo awakeInfo = type.GetMethod("Awake");
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
            MethodInfo awakeInfo = type.GetMethod("Awake");
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
            MethodInfo awakeInfo = type.GetMethod("Awake");
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
            MethodInfo destroyInfo = type.GetMethod("Destroy");
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
