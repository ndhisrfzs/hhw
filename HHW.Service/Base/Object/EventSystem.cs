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
        private static readonly Dictionary<long, Component> allComponents = new Dictionary<long, Component>();
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

        public static void Add(Component component)
        {
            allComponents.Add(component.InstanceId, component);

            Type type = component.GetType();

            if(loadSystems.ContainsKey(type))
            {
                loaders.Enqueue(component.InstanceId);
            }
            if(startSystems.ContainsKey(type))
            {
                starts.Enqueue(component.InstanceId);
            }
            if(updateSystems.ContainsKey(type))
            {
                updates.Enqueue(component.InstanceId);
            }
        }

        public static void Load()
        {
            while(loaders.Count > 0)
            {
                long instanceId = loaders.Dequeue();
                if(!allComponents.TryGetValue(instanceId, out Component component))
                {
                    continue;
                }
                if(component.IsDisposed)
                {
                    continue;
                }
                List<ILoadSystem> iLoadSystems = loadSystems[component.GetType()];
                if(iLoadSystems == null)
                {
                    continue;
                }

                tempLoaders.Enqueue(instanceId);
                foreach (ILoadSystem iLoadSystem in iLoadSystems)
                {
                    try
                    {
                        iLoadSystem.Execute(component);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }

            SwapHelper.Swap(ref loaders, ref tempLoaders);
        }

        public static void Awake(Component component)
        {
            List<IAwakeSystem> iAwakeSystems = awakeSystems[component.GetType()];
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
                    iAwake.Execute(component);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public static void Awake<A>(Component component, A a)
        {
            List<IAwakeSystem> iAwakeSystems = awakeSystems[component.GetType()];
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
                    iAwake.Execute(component, a);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public static void Awake<A, B>(Component component, A a, B b)
        {
            List<IAwakeSystem> iAwakeSystems = awakeSystems[component.GetType()];
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
                    iAwake.Execute(component, a, b);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public static void Awake<A, B, C>(Component component, A a, B b, C c)
        {
            List<IAwakeSystem> iAwakeSystems = awakeSystems[component.GetType()];
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
                    iAwake.Execute(component, a, b, c);
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
                if(!allComponents.TryGetValue(instanceId, out Component component))
                {
                    continue;
                }

                List<IStartSystem> iStartSystems = startSystems[component.GetType()];
                if(iStartSystems == null)
                {
                    continue;
                }

                foreach (IStartSystem iStartSystem in iStartSystems)
                {
                    try
                    {
                        iStartSystem.Execute(component);
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
                Component component;
                if(!allComponents.TryGetValue(instanceId, out component))
                {
                    continue;
                }
                if(component.IsDisposed)
                {
                    continue;
                }

                List<IUpdateSystem> iUpdateSystems = updateSystems[component.GetType()];
                if(iUpdateSystems == null)
                {
                    continue;
                }

                tempUpdates.Enqueue(instanceId);
                foreach (IUpdateSystem iUpdateSystem in iUpdateSystems)
                {
                    try
                    {
                        iUpdateSystem.Execute(component);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }

            SwapHelper.Swap(ref updates, ref tempUpdates);
        }

        public static void Destroy(Component component)
        {
            List<IDestroySystem> iDestroySystems = destroySystems[component.GetType()];
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
                    iDestroySystem.Execute(component);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
