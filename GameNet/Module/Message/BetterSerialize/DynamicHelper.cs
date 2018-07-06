/*注意点：泛型的ConstructorInfo.MetadataToken值一样
 * 
 * 
 */


#if Dynamic
using System;
#if Server
using System.Collections.Concurrent;
#else
using System.Collections.Generic;
#endif
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace GN
{
    /// <summary>
    /// 用于替代反射的动态库
    /// </summary>
    public static class DynamicHelper
    {
        public delegate object DynamicConstructorInfoHandler(object[] parameters);
        public delegate object DynamicFieldGetHandler(object ownerInstance);
        public delegate void DynamicFieldSetHandler(object ownerInstance, object value);
        public delegate object DynamicPropertyGetHandler(object ownerInstance, object[] index);
        public delegate void DynamicPropertySetHandler(object ownerInstance, object value, object[] index);
        /// <summary>
        /// Delegate for calling static method
        /// </summary>
        /// <param name="paramObjs">The parameters passing to the invoking method.</param>
        /// <returns>The return value.</returns>
        public delegate object StaticDynamicMethodHandler(params object[] paramObjs);

        /// <summary>
        /// Delegate for calling non-static method
        /// </summary>
        /// <param name="ownerInstance">The object instance owns the invoking method.</param>
        /// <param name="paramObjs">The parameters passing to the invoking method.</param>
        /// <returns>The return value.</returns>
        public delegate object DynamicMethodHandler(object ownerInstance, params object[] paramObjs);





        private static Type Type_Object = typeof(object);
        private static Type Type_Objects = typeof(object[]);
        private static Type Type_Void = typeof(void);
        private static Type[] FieldGet_P = new Type[] { Type_Object };
        private static Type[] FieldSet_P = new Type[] { Type_Object, Type_Object };
        private static Type[] PropertyGet_P = new Type[] { Type_Object, Type_Objects };
        private static Type[] PropertySet_P = new Type[] { Type_Object, Type_Object, Type_Objects };
        private static Type[] GenericMethod_P = new Type[] { Type_Objects };


        private static Type Type_DCIH = typeof(DynamicConstructorInfoHandler);
        private static Type Type_DFGH = typeof(DynamicFieldGetHandler);
        private static Type Type_DFSH = typeof(DynamicFieldSetHandler);
        private static Type Type_DPGH = typeof(DynamicPropertyGetHandler);
        private static Type Type_DPSH = typeof(DynamicPropertySetHandler);
        private static Type Type_SDM = typeof(StaticDynamicMethodHandler);
        private static Type Type_DM = typeof(DynamicMethodHandler);



#if Server
        private static ConcurrentDictionary<int, DynamicConstructorInfoHandler> CacheConstructor = new ConcurrentDictionary<int, DynamicConstructorInfoHandler>();
        private static ConcurrentDictionary<int, DynamicFieldGetHandler> CacheFieldGet = new ConcurrentDictionary<int, DynamicFieldGetHandler>();
        private static ConcurrentDictionary<int, DynamicFieldSetHandler> CacheFieldSet = new ConcurrentDictionary<int, DynamicFieldSetHandler>();
        private static ConcurrentDictionary<int, DynamicPropertyGetHandler> CachePropertyGet = new ConcurrentDictionary<int, DynamicPropertyGetHandler>();
        private static ConcurrentDictionary<int, DynamicPropertySetHandler> CachePropertySet = new ConcurrentDictionary<int, DynamicPropertySetHandler>();
        //private static ConcurrentDictionary<string, StaticDynamicMethodHandler> CacheStaticDynamicMethod = new ConcurrentDictionary<string, StaticDynamicMethodHandler>();
        private static ConcurrentDictionary<string, DynamicMethodHandler> CacheDynamicMethod = new ConcurrentDictionary<string, DynamicMethodHandler>();
#else
        private static Dictionary<int, DynamicConstructorInfoHandler> CacheConstructor = new Dictionary<int, DynamicConstructorInfoHandler>();
        private static Dictionary<int, DynamicFieldGetHandler> CacheFieldGet = new Dictionary<int, DynamicFieldGetHandler>();
        private static Dictionary<int, DynamicFieldSetHandler> CacheFieldSet = new Dictionary<int, DynamicFieldSetHandler>();
        private static Dictionary<int, DynamicPropertyGetHandler> CachePropertyGet = new Dictionary<int, DynamicPropertyGetHandler>();
        private static Dictionary<int, DynamicPropertySetHandler> CachePropertySet = new Dictionary<int, DynamicPropertySetHandler>();
        //private static Dictionary<string, StaticDynamicMethodHandler> CacheStaticDynamicMethod = new Dictionary<string, StaticDynamicMethodHandler>();
        private static Dictionary<string, DynamicMethodHandler> CacheDynamicMethod = new Dictionary<string, DynamicMethodHandler>();
#endif


        /// <summary>
        /// 根据构造函数和参数，创建对象
        /// </summary>
        /// <param name="constructorInfo">构造函数</param>
        /// <param name="parameters">参数值</param>
        /// <returns>对象</returns>
        public static object CreatInstance(ConstructorInfo constructorInfo, object[] parameters)
        {
            DynamicConstructorInfoHandler handler = null;
            int key = constructorInfo.MetadataToken;

#if Server
            handler = CacheConstructor.GetOrAdd(key, new Func<int, DynamicConstructorInfoHandler>((i) => CreateDynamicConstructorInfoHandler(constructorInfo)));
#else
            lock (CacheConstructor)
            {
                if (CacheConstructor.ContainsKey(key))
                {
                    handler = CacheConstructor[key];
                }
                else
                {
                    handler = CreateDynamicConstructorInfoHandler(constructorInfo);
                    CacheConstructor.Add(key, handler);
                }
            }
#endif

            return handler(parameters);
        }



        /// <summary>
        /// 根据FieldInfo和对象获取对应值
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="obj">FieldInfo所在对象</param>
        /// <returns>对应Field的值</returns>
        public static object GetValue(DynamicFieldInfo fieldInfo, object obj)
        {
            if (fieldInfo.GetField == null)
            {
                fieldInfo.GetField = CreateGetHandler(fieldInfo.TheField);
            } 

            return fieldInfo.GetField(obj);
        }


        /// <summary>
        /// 将指定的值写入到对象的字段中
        /// </summary>
        /// <param name="fieldInfo">字段的FieldInfo</param>
        /// <param name="obj">待写入的对象</param>
        /// <param name="value">将写入的值</param>
        public static void SetValue(DynamicFieldInfo fieldInfo, object obj, object value)
        {
            if (fieldInfo.SetField == null)
            {
                fieldInfo.SetField = CreateSetHandler(fieldInfo.TheField);
            }

            fieldInfo.SetField(obj, value);
        }


        /// <summary>
        /// 根据PropertyInfo和对象获取对应值
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="obj">PropertyInfo所在对象</param>
        /// <param name="index">如果是一个索引Property，请指定索引值，否则，指定null</param>
        /// <returns>返回值</returns>
        public static object GetValue(PropertyInfo propertyInfo, object obj, object[] index)
        {
            DynamicPropertyGetHandler handler = null;
            int key = propertyInfo.MetadataToken;

#if Server
            handler = CachePropertyGet.GetOrAdd(key, new Func<int, DynamicPropertyGetHandler>((i) => CreateGetHandler(propertyInfo)));
#else
            lock (CachePropertyGet)
            {
                if (CachePropertyGet.ContainsKey(key))
                {
                    handler = CachePropertyGet[key];
                }
                else
                {
                    handler = CreateGetHandler(propertyInfo);
                    CachePropertyGet.Add(key, handler);
                }
            }
#endif

            return handler(obj, index);
        }


        /// <summary>
        /// 将指定的值写入到对象的属性中
        /// </summary>
        /// <param name="propertyInfo">属性的PropertyInfo</param>
        /// <param name="obj">待写入的对象</param>
        /// <param name="value">将写入的值</param>
        /// <param name="index">如果是一个索引Property，请指定索引值，否则，指定null</param>
        public static void SetValue(PropertyInfo propertyInfo, object obj, object value, object[] index)
        {
            DynamicPropertySetHandler handler = null;
            int key = propertyInfo.MetadataToken;

#if Server
            handler = CachePropertySet.GetOrAdd(key, new Func<int, DynamicPropertySetHandler>((i) => CreateSetHandler(propertyInfo)));
#else
            lock (CachePropertySet)
            {
                if (CachePropertySet.ContainsKey(key))
                {
                    handler = CachePropertySet[key];
                }
                else
                {
                    handler = CreateSetHandler(propertyInfo);
                    CachePropertySet.Add(key, handler);
                }
            }
#endif

            handler(obj, value, index);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="ownerInstance"></param>
        /// <param name="paramObjs"></param>
        /// <returns></returns>
        public static object InvokeMethod(MethodInfo methodInfo, object ownerInstance, object[] paramObjs)
        {
            DynamicMethodHandler handler = null;

            StringBuilder sb = new StringBuilder();
            sb.Append(methodInfo.DeclaringType.FullName);
            sb.Append("|");
            sb.Append(methodInfo.Name);

            Type[] genericParameterTypes = null;
            if (methodInfo.IsGenericMethod)
            {
                genericParameterTypes = methodInfo.GetGenericArguments();
            }

            if (genericParameterTypes != null)
            {
                for (int i = 0; i < genericParameterTypes.Length; ++i)
                {
                    sb.Append("|");
                    sb.Append(genericParameterTypes[i].ToString());
                }
            }

            string key = sb.ToString();
#if Server
            handler = CacheDynamicMethod.GetOrAdd(key, new Func<string, DynamicMethodHandler>((s) => CreatMethodDelegate(methodInfo, genericParameterTypes)));

#else
            lock(CacheDynamicMethod)
            {
                if (CacheDynamicMethod.ContainsKey(key))
                {
                    handler = CacheDynamicMethod[key];
                }
                else
                {
                    handler = CreatMethodDelegate(methodInfo, genericParameterTypes);
                    CacheDynamicMethod.Add(key, handler);
                }
            }
#endif
            //调用相应的方法
            return handler(ownerInstance, paramObjs);
        }



        /// <summary>
        /// 创建构造函数
        /// </summary>
        /// <param name="constructorInfo"></param>
        /// <returns></returns>
        private static DynamicConstructorInfoHandler CreateDynamicConstructorInfoHandler(ConstructorInfo constructorInfo)
        {
            int argIndex = 0;

            DynamicMethod dynamicMethod = new DynamicMethod("D",
                MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard, Type_Object, FieldGet_P, constructorInfo.DeclaringType, true);
            ILGenerator generator = dynamicMethod.GetILGenerator();

            foreach (ParameterInfo parainfo in constructorInfo.GetParameters())
            {
                generator.Emit(OpCodes.Ldarg_0);
                if (argIndex > 8)
                    generator.Emit(OpCodesFactory.GetLdc_I4(argIndex), argIndex);
                else
                    generator.Emit(OpCodesFactory.GetLdc_I4(argIndex));
                generator.Emit(OpCodes.Ldelem_Ref);
                OpCodesFactory.UnboxIfNeeded(generator, parainfo.ParameterType);
                argIndex++;
            }
            generator.Emit(OpCodes.Newobj, constructorInfo);
            generator.Emit(OpCodes.Ret);
            return (DynamicConstructorInfoHandler)dynamicMethod.CreateDelegate(Type_DCIH);
        }



        /// <summary>
        /// 创建Field的动态方法get
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        private static DynamicFieldGetHandler CreateGetHandler(FieldInfo fieldInfo)
        {
            DynamicMethod dynamicGet = new DynamicMethod("GF", Type_Object, FieldGet_P, fieldInfo.DeclaringType, true);
            ILGenerator getGenerator = dynamicGet.GetILGenerator();

            getGenerator.Emit(OpCodes.Ldarg_0);
            getGenerator.Emit(OpCodes.Ldfld, fieldInfo);
            OpCodesFactory.BoxIfNeeded(getGenerator, fieldInfo.FieldType);
            getGenerator.Emit(OpCodes.Ret);

            return (DynamicFieldGetHandler)dynamicGet.CreateDelegate(Type_DFGH);
        }

        /// <summary>
        /// 创建Field的动态方法set
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        private static DynamicFieldSetHandler CreateSetHandler(FieldInfo fieldInfo)
        {
            DynamicMethod dynamicSet = new DynamicMethod("SF", Type_Void, FieldSet_P, fieldInfo.DeclaringType, true);
            ILGenerator setGenerator = dynamicSet.GetILGenerator();

            setGenerator.Emit(OpCodes.Ldarg_0);
            setGenerator.Emit(OpCodes.Ldarg_1);
            OpCodesFactory.UnboxIfNeeded(setGenerator, fieldInfo.FieldType);
            setGenerator.Emit(OpCodes.Stfld, fieldInfo);
            setGenerator.Emit(OpCodes.Ret);



            return (DynamicFieldSetHandler)dynamicSet.CreateDelegate(Type_DFSH);
        }



        /// <summary>
        /// 创建PropertyInfo的动态方法get
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private static DynamicPropertyGetHandler CreateGetHandler(PropertyInfo propertyInfo)
        {
            MethodInfo getMethodInfo = propertyInfo.GetGetMethod(true);
            int argIndex = 0;

            DynamicMethod dynamicGet = new DynamicMethod("GP", Type_Object, PropertyGet_P, propertyInfo.DeclaringType, true);
            ILGenerator getGenerator = dynamicGet.GetILGenerator();

            getGenerator.Emit(OpCodes.Ldarg_0);
            foreach (ParameterInfo parainfo in getMethodInfo.GetParameters())
            {
                getGenerator.Emit(OpCodes.Ldarg_1);
                if (argIndex > 8)
                    getGenerator.Emit(OpCodesFactory.GetLdc_I4(argIndex), argIndex);
                else
                    getGenerator.Emit(OpCodesFactory.GetLdc_I4(argIndex));
                getGenerator.Emit(OpCodes.Ldelem_Ref);
                OpCodesFactory.UnboxIfNeeded(getGenerator, parainfo.ParameterType);
                argIndex++;
            }
            getGenerator.Emit(OpCodes.Callvirt, getMethodInfo);
            OpCodesFactory.BoxIfNeeded(getGenerator, getMethodInfo.ReturnType);
            getGenerator.Emit(OpCodes.Ret);

            return (DynamicPropertyGetHandler)dynamicGet.CreateDelegate(Type_DPGH);
        }


        /// <summary>
        /// 创建PropertyInfo的动态方法set
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private static DynamicPropertySetHandler CreateSetHandler(PropertyInfo propertyInfo)
        {
            MethodInfo setMethodInfo = propertyInfo.GetSetMethod(true);
            int argCount = setMethodInfo.GetParameters().Length;
            int argIndex = 0;

            DynamicMethod dynamicSet = new DynamicMethod("SP", Type_Void, PropertySet_P, propertyInfo.DeclaringType, true);
            ILGenerator setGenerator = dynamicSet.GetILGenerator();

            setGenerator.Emit(OpCodes.Ldarg_0);
            foreach (ParameterInfo parainfo in setMethodInfo.GetParameters())
            {
                if (argIndex + 1 >= argCount)
                    break;

                setGenerator.Emit(OpCodes.Ldarg_2);
                if (argIndex > 8)
                    setGenerator.Emit(OpCodesFactory.GetLdc_I4(argIndex), argIndex);
                else
                    setGenerator.Emit(OpCodesFactory.GetLdc_I4(argIndex));
                setGenerator.Emit(OpCodes.Ldelem_Ref);
                OpCodesFactory.UnboxIfNeeded(setGenerator, parainfo.ParameterType);
                argIndex++;
            }
            setGenerator.Emit(OpCodes.Ldarg_1);
            OpCodesFactory.UnboxIfNeeded(setGenerator, setMethodInfo.GetParameters()[argIndex].ParameterType);
            setGenerator.Emit(OpCodes.Call, setMethodInfo);
            setGenerator.Emit(OpCodes.Ret);

            return (DynamicPropertySetHandler)dynamicSet.CreateDelegate(Type_DPSH);
        }




        private static DynamicMethodHandler CreatMethodDelegate(MethodInfo genericMethodInfo, Type[] genericParameterTypes)
        {
            MethodInfo makeGenericMethodInfo;
            if (genericParameterTypes != null && genericParameterTypes.Length > 0)
            {
                makeGenericMethodInfo = genericMethodInfo.MakeGenericMethod(genericParameterTypes);
            }
            else
            {
                makeGenericMethodInfo = genericMethodInfo;
            }

            DynamicMethod dynamicMethod = new DynamicMethod("DM", Type_Object, PropertyGet_P, makeGenericMethodInfo.DeclaringType.Module);
            ILGenerator il = dynamicMethod.GetILGenerator();
            ParameterInfo[] ps = makeGenericMethodInfo.GetParameters();
            Type[] paramTypes = new Type[ps.Length];
            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                    paramTypes[i] = ps[i].ParameterType.GetElementType();
                else
                    paramTypes[i] = ps[i].ParameterType;
            }
            LocalBuilder[] locals = new LocalBuilder[paramTypes.Length];

            for (int i = 0; i < paramTypes.Length; i++)
            {
                locals[i] = il.DeclareLocal(paramTypes[i], true);
            }
            for (int i = 0; i < paramTypes.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_1);
                OpCodesFactory.EmitFastInt(il, i);
                il.Emit(OpCodes.Ldelem_Ref);
                OpCodesFactory.EmitCastToReference(il, paramTypes[i]);
                il.Emit(OpCodes.Stloc, locals[i]);
            }
            if (!makeGenericMethodInfo.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
            }
            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                    il.Emit(OpCodes.Ldloca_S, locals[i]);
                else
                    il.Emit(OpCodes.Ldloc, locals[i]);
            }
            if (makeGenericMethodInfo.IsStatic)
                il.EmitCall(OpCodes.Call, makeGenericMethodInfo, null);
            else
                il.EmitCall(OpCodes.Callvirt, makeGenericMethodInfo, null);
            if (makeGenericMethodInfo.ReturnType == typeof(void))
                il.Emit(OpCodes.Ldnull);
            else
                OpCodesFactory.BoxIfNeeded(il, makeGenericMethodInfo.ReturnType);

            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                {
                    il.Emit(OpCodes.Ldarg_1);
                    OpCodesFactory.EmitFastInt(il, i);
                    il.Emit(OpCodes.Ldloc, locals[i]);
                    if (locals[i].LocalType.IsValueType)
                        il.Emit(OpCodes.Box, locals[i].LocalType);
                    il.Emit(OpCodes.Stelem_Ref);
                }
            }

            il.Emit(OpCodes.Ret);
            return (DynamicMethodHandler)dynamicMethod.CreateDelegate(Type_DM);
        }
    }
}
#endif
