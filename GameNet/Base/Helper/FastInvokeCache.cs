using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace GN
{
    public static class FastInvokeCache
    {
        static ConcurrentDictionary<Type, ConcurrentDictionary<MethodInfo, FastInvoke.FastInvokeHandler>> invokeCache = new ConcurrentDictionary<Type, ConcurrentDictionary<MethodInfo, FastInvoke.FastInvokeHandler>>();
        public static FastInvoke.FastInvokeHandler GetMethod(Type type, string method, params Type[] args)
        {
            FastInvoke.FastInvokeHandler handler = null;
            ConcurrentDictionary<MethodInfo, FastInvoke.FastInvokeHandler> invokes = null;
            MethodInfo methodInfo = type.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, args, null);
            if (methodInfo != null)
            {
                if (invokeCache.TryGetValue(type, out invokes))
                {
                    if (invokes.TryGetValue(methodInfo, out handler))
                    {
                        return handler;
                    }
                }

                handler = FastInvoke.GetMethodInvoker(methodInfo);
                if (handler != null && invokes == null)
                {
                    invokes = new ConcurrentDictionary<MethodInfo, FastInvoke.FastInvokeHandler>();
                    invokeCache.TryAdd(type, invokes);
                }

                invokes.TryAdd(methodInfo, handler);
            }
            return handler;
        }
    }
}
