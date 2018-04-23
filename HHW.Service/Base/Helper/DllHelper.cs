using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HHW.Service.Base.Helper
{
    public static class DllHelper
    {
        public static Assembly GetHotfixAssembly()
        {
            byte[] dllBytes = File.ReadAllBytes("./Hotfix.dll");
            byte[] pdbBytes = File.ReadAllBytes("./Hotfix.pdb");
            Assembly assembly = Assembly.Load(dllBytes, pdbBytes);
            return assembly;
        }

        public static Type[] GetMonoTypes(Assembly[] assemblies)
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
