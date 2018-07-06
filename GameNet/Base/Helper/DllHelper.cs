using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GN
{
    public static class DllHelper
    {
        public static Assembly GetLogicAssembly()
        {
            byte[] dllBytes = File.ReadAllBytes("./Logic.dll");
#if __MonoCS__
			byte[] pdbBytes = File.ReadAllBytes("./Logic.dll.mdb");
#else
            byte[] pdbBytes = File.ReadAllBytes("./Logic.pdb");
#endif
            Assembly assembly = Assembly.Load(dllBytes, pdbBytes);
            return assembly;
        }

        public static Type[] GetMonoTypes(params Assembly[] assemblies)
        {
            List<Type> types = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                types.AddRange(assembly.GetTypes());
            }
            return types.ToArray();
        }
    }
}
