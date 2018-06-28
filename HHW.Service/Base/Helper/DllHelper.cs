﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HHW.Service
{
    public static class DllHelper
    {
        public static Assembly GetLogicAssembly()
        {
            byte[] dllBytes = File.ReadAllBytes("Logic.dll");
            byte[] pdbBytes = File.ReadAllBytes("Logic.pdb");
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
