using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HHW.Service
{
    public enum DLLType
    {
        Model,
        Hotfix,
        Editor,
    }
    public static class EventSystem
    {
        private static readonly Dictionary<long, Object> allComponents = new Dictionary<long, Object>();
        private static readonly Dictionary<DLLType, Assembly> assemblies = new Dictionary<DLLType, Assembly>();
        private static readonly UnOrderMultiMap<Type, IAwakeSystem> awakeSystems = new UnOrderMultiMap<Type, IAwakeSystem>();
        private static readonly UnOrderMultiMap<Type, IStartSystem> startSystems = new UnOrderMultiMap<Type, IStartSystem>();
        private static readonly UnOrderMultiMap<Type, ILoadSystem> loadSystems = new UnOrderMultiMap<Type, ILoadSystem>();
        private static readonly UnOrderMultiMap<Type, IUpdateSystem> updateSystems = new UnOrderMultiMap<Type, IUpdateSystem>();
        private static readonly UnOrderMultiMap<Type, IDestroySystem> destroySystems = new UnOrderMultiMap<Type, IDestroySystem>();

        private static readonly Queue<long> starts = new Queue<long>();

        private static Queue<long> loaders = new Queue<long>();
        private static Queue<long> tempLoaders = new Queue<long>();

        private static Queue<long> updates = new Queue<long>();
        private static Queue<long> tempUpdates = new Queue<long>();

        public static Assembly[] GetAll()
        {
            return assemblies.Values.ToArray();
        }

        public static void Add(DLLType dllType, Assembly assembly)
        {
            assemblies[dllType] = assembly;

            awakeSystems.Clear();
            startSystems.Clear();
            loadSystems.Clear();
            updateSystems.Clear();
            destroySystems.Clear();

            Type[] types = DllHelper.GetMonoTypes(GetAll());
            foreach (Type type in types)
            {
                if (type.IsInterface || type.IsAbstract || !typeof(ISystem).IsAssignableFrom(type))
                {
                    continue;
                }

                object obj = Activator.CreateInstance(type);

                ILoadSystem loadSystem = obj as ILoadSystem;
                if(loadSystem != null)
                {
                    loadSystems.Add(loadSystem.Type(), loadSystem);
                }

                IAwakeSystem awakeSystem = obj as IAwakeSystem;
                if(awakeSystem != null)
                {
                    awakeSystems.Add(awakeSystem.Type(), awakeSystem);
                }

                IStartSystem startSystem = obj as IStartSystem;
                if(startSystem != null)
                {
                    startSystems.Add(startSystem.Type(), startSystem);
                }

                IUpdateSystem updateSystem = obj as IUpdateSystem;
                if(updateSystem != null)
                {
                    updateSystems.Add(updateSystem.Type(), updateSystem);
                }

                IDestroySystem destroySystem = obj as IDestroySystem;
                if(destroySystem != null)
                {
                    destroySystems.Add(destroySystem.Type(), destroySystem);
                }
            }

            Load();
        }

        public static void Add(Object obj)
        {
            allComponents.Add(obj.id, obj);

            Type type = obj.GetType();

            if(loadSystems.ContainsKey(type))
            {
                loaders.Enqueue(obj.id);
            }
            if(startSystems.ContainsKey(type))
            {
                starts.Enqueue(obj.id);
            }
            if(updateSystems.ContainsKey(type))
            {
                updates.Enqueue(obj.id);
            }
        }

        public static void Load()
        {
            while(loaders.Count > 0)
            {
                long instanceId = loaders.Dequeue();
                Object obj;
                if(!allComponents.TryGetValue(instanceId, out obj))
                {
                    continue;
                }
                if(obj.IsDisposed)
                {
                    continue;
                }
                List<ILoadSystem> iLoadSystems = loadSystems[obj.GetType()];
                if(iLoadSystems == null)
                {
                    continue;
                }

                tempLoaders.Enqueue(instanceId);
                foreach (ILoadSystem iLoadSystem in iLoadSystems)
                {
                    try
                    {
                        iLoadSystem.Execute(obj);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }

            SwapHelper.Swap(ref loaders, ref tempLoaders);
        }

        public static void Awake(Object obj)
        {
            List<IAwakeSystem> iAwakeSystems = awakeSystems[obj.GetType()];
            if(iAwakeSystems == null)
            {
                return;
            }

            foreach (IAwakeSystem iAwakeSystem in iAwakeSystems)
            {
                if(iAwakeSystem == null)
                {
                    continue;
                }

                IAwake iAwake = iAwakeSystem as IAwake;
                if(iAwake == null)
                {
                    continue;
                }

                try
                {
                    iAwake.Execute(obj);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public static void Awake<A>(Object obj, A a)
        {
            List<IAwakeSystem> iAwakeSystems = awakeSystems[obj.GetType()];
            if(iAwakeSystems == null)
            {
                return;
            }

            foreach (IAwakeSystem iAwakeSystem in iAwakeSystems)
            {
                if(iAwakeSystem == null)
                {
                    continue;
                }

                IAwake<A> iAwake = iAwakeSystem as IAwake<A>;
                if (iAwake == null)
                {
                    continue;
                }

                try
                {
                    iAwake.Execute(obj, a);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public static void Awake<A, B>(Object obj, A a, B b)
        {
            List<IAwakeSystem> iAwakeSystems = awakeSystems[obj.GetType()];
            if (iAwakeSystems == null)
            {
                return;
            }

            foreach (IAwakeSystem iAwakeSystem in iAwakeSystems)
            {
                if (iAwakeSystem == null)
                {
                    continue;
                }

                IAwake<A, B> iAwake = iAwakeSystem as IAwake<A, B>;
                if (iAwake == null)
                {
                    continue;
                }

                try
                {
                    iAwake.Execute(obj, a, b);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public static void Awake<A, B, C>(Object obj, A a, B b, C c)
        {
            List<IAwakeSystem> iAwakeSystems = awakeSystems[obj.GetType()];
            if (iAwakeSystems == null)
            {
                return;
            }

            foreach (IAwakeSystem iAwakeSystem in iAwakeSystems)
            {
                if (iAwakeSystem == null)
                {
                    continue;
                }

                IAwake<A, B, C> iAwake = iAwakeSystem as IAwake<A, B, C>;
                if (iAwake == null)
                {
                    continue;
                }

                try
                {
                    iAwake.Execute(obj, a, b, c);
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
                long instanceId = starts.Dequeue();
                Object obj;
                if(!allComponents.TryGetValue(instanceId, out obj))
                {
                    continue;
                }

                List<IStartSystem> iStartSystems = startSystems[obj.GetType()];
                if(iStartSystems == null)
                {
                    continue;
                }

                foreach (IStartSystem iStartSystem in iStartSystems)
                {
                    try
                    {
                        iStartSystem.Execute(obj);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }

        public static void Update()
        {
            Start();

            while(updates.Count > 0)
            {
                long instanceId = updates.Dequeue();
                Object obj;
                if(!allComponents.TryGetValue(instanceId, out obj))
                {
                    continue;
                }
                if(obj.IsDisposed)
                {
                    continue;
                }

                List<IUpdateSystem> iUpdateSystems = updateSystems[obj.GetType()];
                if(iUpdateSystems == null)
                {
                    continue;
                }

                tempUpdates.Enqueue(instanceId);
                foreach (IUpdateSystem iUpdateSystem in iUpdateSystems)
                {
                    try
                    {
                        iUpdateSystem.Execute(obj);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }

            SwapHelper.Swap(ref updates, ref tempUpdates);
        }

        public static void Destroy(Object obj)
        {
            List<IDestroySystem> iDestroySystems = destroySystems[obj.GetType()];
            if(iDestroySystems == null)
            {
                return;
            }

            foreach (IDestroySystem iDestroySystem in iDestroySystems)
            {
                if(iDestroySystem == null)
                {
                    continue;
                }

                try
                {
                    iDestroySystem.Execute(obj);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
