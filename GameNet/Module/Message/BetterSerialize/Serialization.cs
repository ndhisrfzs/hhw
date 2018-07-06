/* 不能反射接口成员，支持大多数基础值类型，极其数组，支持IList<>泛型类型，支持IDictionary<>泛型类型，支持无限层嵌套
 * 
 * 
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace GN
{
    /// <summary>
    /// 支持无限层级、null值、0大小的数组、中间存在“\0”并且处于任意位置和数量字符串、空字符串、
    /// 支持数据库中所有的基础值类型，并对其数组进行了直接优化、支持Nullable泛型类型，支持IList泛型类型，支持IDictionary泛型类型、
    /// 最佳二进制尺寸、服务器版使用IL代码进行了优化
    /// </summary>
    public static class Serialization
    {
        public static BindingFlags AllInstanceBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private const byte Byte_Zero = 0;

        private const byte Byte_One = 0xff;

        private const ushort DefaultFieldLen = 0;


        public static ArgumentException Exception_CannotConvert = new ArgumentException();


        private static Dictionary<Type, DynamicFieldInfo[]> FI_Lookup = new Dictionary<Type, DynamicFieldInfo[]>();


        /// <summary>
        /// 获取所有字段，包括继承来的父类字段
        /// </summary>
        /// <param name="TheType"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        private static List<FieldInfo> GetAllFields(this Type TheType, BindingFlags bindingFlags)
        {
            List<FieldInfo> fieldInfoes = new List<FieldInfo>();
            Type thisType = TheType;
            while (thisType.IsSubclassOf(typeof(object)))
            {
                fieldInfoes.AddRange(thisType.GetFields(bindingFlags));
                thisType = thisType.BaseType;
            }

            return fieldInfoes;
        }
        /// <summary>
        /// 序列化一个对象到Byte数组，无法序列化，将抛出ArgumentException异常
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="writeToBytes">将要在这个数组基础上开始写入，指定Null表示新建一个数组</param>
        /// <param name="startIndex">将要从数组的哪个位置开始写入</param>
        /// <param name="maxWriteSize">最大允许的写入长度</param>
        /// <param name="fullLengthSchema">是否完整包含每个子对象的长度</param>
        /// <returns>返回有效大小</returns>
        public static int SerializeTo(object obj, ref byte[] writeToBytes, int startIndex, int maxWriteSize, bool fullLengthSchema = true)
        {
            //try
            {
                bool isFieldNullable = false;

                //数据块
                ZipBytesWriter bwData;
                if (writeToBytes != null)
                {
                    bwData = new ZipBytesWriter(writeToBytes, startIndex, maxWriteSize);
                }
                else
                {
                    bwData = new ZipBytesWriter(maxWriteSize);
                }

                //写标识头
                bwData.Write((fullLengthSchema) ? Byte_One : Byte_Zero);

                if (obj == null)
                {
                    if (fullLengthSchema)
                    {
                        bwData.Write((ushort)1);
                    }
                    bwData.Write(Byte_Zero);
                    writeToBytes = bwData.Bytes;
                    return bwData.EffectiveBytesLength;
                }
                int metablockStart = 0;
                int metablockLen = 0;
                Stack<SerializeInfo> bufStack = new Stack<SerializeInfo>();

                bufStack.Push(new SerializeInfo(obj, isFieldNullable, bwData.EffectiveBytesLength));
                if (fullLengthSchema)
                {
                    bwData.Write(DefaultFieldLen);
                }
                while (bufStack.Count > 0)
                {
                    SerializeInfo nowSerializeInfo = bufStack.Peek();

                    if (nowSerializeInfo.TheObject == null)
                    {
                        bwData.WriteNullHead(true); //写入null
                        bufStack.Pop();
                        if (fullLengthSchema)
                        {
                            metablockStart = nowSerializeInfo.ByteStartIndexOrEndPoint;
                            metablockLen = bwData.EffectiveBytesLength - metablockStart - 2;
                            ZipBytesWriter.WriteAtUshort(bwData.Bytes, metablockStart, (ushort)metablockLen);
                        }
                    }
                    else
                    {
                        if (nowSerializeInfo.TheType == null)  //未检查过
                        {
                            if (nowSerializeInfo.IsNullable)
                            {
                                bwData.WriteNullHead(false);
                            }

                            nowSerializeInfo.TheType = nowSerializeInfo.TheObject.GetType();

                            //是基础元素
                            if (BytesHelper.TryWriteBytes(nowSerializeInfo.TheObject, nowSerializeInfo.TheType, bwData))
                            {
                                bufStack.Pop();
                                if (fullLengthSchema)
                                {
                                    metablockStart = nowSerializeInfo.ByteStartIndexOrEndPoint;
                                    metablockLen = bwData.EffectiveBytesLength - metablockStart - 2;
                                    if (metablockLen > ushort.MaxValue)
                                    {
                                        throw new Exception("序列化的子字段大小超出65535:" + nowSerializeInfo.FieldInfos[nowSerializeInfo.Index - 1].FieldName);
                                    }
                                    ZipBytesWriter.WriteAtUshort(bwData.Bytes, metablockStart, (ushort)metablockLen);
                                }
                            }
                            else //不是基础元素
                            {
                                if (nowSerializeInfo.TheType.IsArray)
                                {
                                    nowSerializeInfo.IsArray = true;
                                    nowSerializeInfo.Length_FieldInfo = (nowSerializeInfo.TheObject as Array).Length;

                                    //写入数组长度
                                    bwData.WriteArrayHead(nowSerializeInfo.Length_FieldInfo);
                                }
                                else if (nowSerializeInfo.TheType.GetInterface("IList`1", false) != null)
                                {
                                    nowSerializeInfo.IsIList = true;

                                    nowSerializeInfo.Length_FieldInfo = (nowSerializeInfo.TheObject as IList).Count;

                                    //写入数组长度
                                    bwData.WriteArrayHead(nowSerializeInfo.Length_FieldInfo);
                                }
                                else if (nowSerializeInfo.TheType.GetInterface("IDictionary`2", false) != null)
                                {
                                    nowSerializeInfo.IsIDictionary = true;

                                    IDictionary id = nowSerializeInfo.TheObject as IDictionary;
                                    int dLength = id.Count;
                                    nowSerializeInfo.Length_FieldInfo = dLength * 2;

                                    object[] kvs = new object[nowSerializeInfo.Length_FieldInfo];
                                    id.Keys.CopyTo(kvs, 0);
                                    id.Values.CopyTo(kvs, dLength);

                                    nowSerializeInfo.TheObject = kvs;

                                    //写入数组长度
                                    bwData.WriteArrayHead(nowSerializeInfo.Length_FieldInfo);
                                }
                                else
                                {
                                    bwData.WriteNullHead(false);

                                    DynamicFieldInfo[] fis;

                                    lock (FI_Lookup)
                                    {
                                        if (FI_Lookup.ContainsKey(nowSerializeInfo.TheType))
                                        {
                                            fis = FI_Lookup[nowSerializeInfo.TheType];
                                        }
                                        else
                                        {
                                            PropertyInfo[] fi = nowSerializeInfo.TheType.GetProperties(AllInstanceBindingFlags).OrderBy(c => c.Name).ToArray();
                                            List<DynamicFieldInfo> tempFis = new List<DynamicFieldInfo>();
                                            for (int i = 0; i < fi.Length; i++)
                                            {
                                                PropertyInfo fiNow = fi[i];
                                                object[] at = fiNow.GetCustomAttributes(typeof(System.NonSerializedAttribute), true);
                                                if (at.Length == 0)
                                                {
                                                    tempFis.Add(new DynamicFieldInfo() { TheField = fiNow });
                                                }
                                            }
                                            fis = tempFis.ToArray();
                                            FI_Lookup.Add(nowSerializeInfo.TheType, fis);
                                        }
                                    }


                                    nowSerializeInfo.FieldInfos = fis;
                                    nowSerializeInfo.Length_FieldInfo = fis.Length;
                                }



                                if (nowSerializeInfo.Length_FieldInfo == 0)  //虽然不是基础元素，但没有可序列化的字段
                                {
                                    bufStack.Pop();
                                    if (fullLengthSchema)
                                    {
                                        metablockStart = nowSerializeInfo.ByteStartIndexOrEndPoint;
                                        metablockLen = bwData.EffectiveBytesLength - metablockStart - 2;
                                        if (metablockLen > ushort.MaxValue)
                                        {
                                            throw new Exception("序列化的子字段大小超出65535:" + nowSerializeInfo.FieldInfos[nowSerializeInfo.Index - 1].FieldName);
                                        }
                                        ZipBytesWriter.WriteAtUshort(bwData.Bytes, metablockStart, (ushort)metablockLen);
                                    }
                                }
                                else
                                {
                                    nowSerializeInfo.Index = 0;

                                    isFieldNullable = false;
                                    object innerObject = nowSerializeInfo.GetInnerField(ref isFieldNullable);
                                    nowSerializeInfo.Index++;

                                    bufStack.Push(new SerializeInfo(innerObject, isFieldNullable, bwData.EffectiveBytesLength));
                                    if (fullLengthSchema)
                                    {
                                        bwData.Write(DefaultFieldLen);
                                    }
                                }
                            }
                        }
                        else //上次检查过
                        {
                            if (nowSerializeInfo.Length_FieldInfo == nowSerializeInfo.Index)  //检查完了
                            {
                                bufStack.Pop();
                                if (fullLengthSchema)
                                {
                                    metablockStart = nowSerializeInfo.ByteStartIndexOrEndPoint;
                                    metablockLen = bwData.EffectiveBytesLength - metablockStart - 2;
                                    if (metablockLen > ushort.MaxValue)
                                    {
                                        throw new Exception("序列化的子字段大小超出65535:" + nowSerializeInfo.FieldInfos[nowSerializeInfo.Index - 1].FieldName);
                                    }
                                    ZipBytesWriter.WriteAtUshort(bwData.Bytes, metablockStart, (ushort)metablockLen);
                                }
                            }
                            else
                            {
                                isFieldNullable = false;
                                object innerObject = nowSerializeInfo.GetInnerField(ref isFieldNullable);
                                nowSerializeInfo.Index++;

                                bufStack.Push(new SerializeInfo(innerObject, isFieldNullable, bwData.EffectiveBytesLength));
                                if (fullLengthSchema)
                                {
                                    bwData.Write(DefaultFieldLen);
                                }
                            }
                        }
                    }
                }

                writeToBytes = bwData.Bytes;
                return bwData.EffectiveBytesLength;
            }
            //catch (Exception ee)
            {
                //throw new SerializeException(obj.GetType().ToString() + "Data bytes and type not match:" + ee.Message);
            }
        }



        public static int SerializeTo(object obj, ref byte[] writeToBytes, int startIndex, bool fullLengthSchema = true)
        {
            return SerializeTo(obj, ref writeToBytes, startIndex, Int32.MaxValue / 2, fullLengthSchema);
        }



        /// <summary>
        /// 反序列化Byte数组到一个对象，无法反序列化，将抛出ArgumentException异常
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="bytesValue">二进制数据</param>
        /// <param name="startIndex">开始执行的位置</param>
        /// <param name="maxReadSize">指定支持最大反序列化的数据大小</param>
        /// <param name="readLength">实际反序列化时读取的元素个数</param>
        /// <returns>对象</returns>
        public static object DeserializeFrom(Type objectType, byte[] bytesValue, int startIndex, int maxReadSize, out int readLength)
        {
            try
            {
                ZipBytesReader brData = new ZipBytesReader(startIndex, maxReadSize, bytesValue);

                int point = startIndex;
                object fatherObject = null;

                object innerObject = null;

                int readEnd = 0;
                byte isNull = 0;
                int totalLength = 0;
                int nextSeekTo = 0;
                ushort metablockLen = 0;


                //新的字段信息，一般只保存类型和父类
                SerializeInfo innerSerializeInfo;

                Stack<SerializeInfo> bufStack = new Stack<SerializeInfo>();

                bool fullLengthSchema = false;
                if (brData.ReadByte() != Byte_Zero)
                {
                    fullLengthSchema = true;

                    metablockLen = brData.TryReadMetaBlockLength();
                    totalLength = brData.Position + metablockLen;
                }

                bufStack.Push(new SerializeInfo(objectType, totalLength));
                
                while (bufStack.Count > 0)
                {
                    //当前的字段信息
                    SerializeInfo nowSerializeInfo = bufStack.Peek();
                    readEnd = nowSerializeInfo.ByteStartIndexOrEndPoint;

                    if (nowSerializeInfo.TheObject == null)  //未检查过
                    {
                        //专门处理Nullable类型
                        if (nowSerializeInfo.TheType.IsGenericType)
                        {
                            if (nowSerializeInfo.TheType.GetGenericTypeDefinition() == SerializeInfo.Type_Nullable)
                            {
                                isNull = brData.ReadByte();
                                if (isNull == Byte_Zero)  //为null
                                {
                                    nowSerializeInfo.FatherSerializeInfo.Index++;
                                    bufStack.Pop();
                                    if (fullLengthSchema)
                                    {
                                        if (readEnd >= totalLength)
                                        {
                                            break;
                                        }
                                        brData.SeekNextTo(readEnd);
                                    }
                                    continue;
                                }
                                else
                                {
                                    nowSerializeInfo.TheType = nowSerializeInfo.TheType.GetGenericArguments()[0];
                                }
                            }
                        }

                        //基础类
                        if (BytesHelper.TryGetValue(nowSerializeInfo.TheType, out innerObject, brData))
                        {
                            if (nowSerializeInfo.FatherSerializeInfo != null)  //有可能直接就是个基础类型
                            {
                                if (nowSerializeInfo.FatherSerializeInfo.IsIDictionary)
                                {
                                    if (nowSerializeInfo.FatherSerializeInfo.Index < nowSerializeInfo.FatherSerializeInfo.Length_FieldInfo / 2)
                                    {
                                        nowSerializeInfo.FatherSerializeInfo.Keys[nowSerializeInfo.FatherSerializeInfo.Index] = innerObject;
                                    }
                                    else
                                    {
                                        nowSerializeInfo.FatherSerializeInfo.SetInnerField(innerObject);
                                    }
                                }
                                else
                                {
                                    nowSerializeInfo.FatherSerializeInfo.SetInnerField(innerObject);
                                }
                                nowSerializeInfo.FatherSerializeInfo.Index++;
                            }
                            else  //有可能直接就是个基础类型
                            {
                                fatherObject = innerObject;
                            }
                            bufStack.Pop();
                            if (fullLengthSchema)
                            {
                                if (readEnd >= totalLength)
                                {
                                    break;
                                }
                                if (readEnd > brData.Position)
                                {
                                    brData.SeekNextTo(readEnd);
                                }
                            }
                        }
                        else  //非基础类
                        {
                            isNull = brData.ReadByte();

                            if (isNull == Byte_Zero)  //为null
                            {
                                if (nowSerializeInfo.FatherSerializeInfo == null)  //直接就是null
                                {
                                    fatherObject = null;
                                }
                                else
                                {
                                    if (nowSerializeInfo.FatherSerializeInfo.IsIList)
                                    {
                                        nowSerializeInfo.FatherSerializeInfo.Index++;
                                        nowSerializeInfo.FatherSerializeInfo.SetInnerField(null);
                                    }
                                    else if (nowSerializeInfo.FatherSerializeInfo.IsIDictionary)
                                    {
                                        nowSerializeInfo.FatherSerializeInfo.SetInnerField(null);
                                        nowSerializeInfo.FatherSerializeInfo.Index++;
                                    }
                                    else
                                    {
                                        nowSerializeInfo.FatherSerializeInfo.Index++;
                                    }
                                }
                                bufStack.Pop();
                                if (fullLengthSchema)
                                {
                                    if (readEnd >= totalLength)
                                    {
                                        break;
                                    }
                                    if (readEnd > brData.Position)
                                    {
                                        brData.SeekNextTo(readEnd);
                                    }
                                }
                            }
                            else
                            {
                                if (nowSerializeInfo.TheType.IsArray)
                                {
                                    nowSerializeInfo.IsArray = true;
                                    nowSerializeInfo.TheElementType = nowSerializeInfo.TheType.GetElementType();
                                    nowSerializeInfo.Length_FieldInfo = (int)brData.ReadUInt32();
                                }
                                else if (nowSerializeInfo.TheType.GetInterface("IList`1", false) != null)
                                {
                                    nowSerializeInfo.IsIList = true;

                                    object[] ubas = nowSerializeInfo.TheType.GetCustomAttributes(false);
                                    if (ubas.Length > 0)
                                    {
                                        UseBaseArgument uba = ubas[0] as UseBaseArgument;
                                        if (uba != null)
                                        {
                                            nowSerializeInfo.TheElementType = nowSerializeInfo.TheType.BaseType.GetGenericArguments()[0];
                                        }
                                        else
                                        {
                                            nowSerializeInfo.TheElementType = nowSerializeInfo.TheType.GetGenericArguments()[0];
                                        }
                                    }
                                    else
                                    {
                                        nowSerializeInfo.TheElementType = nowSerializeInfo.TheType.GetGenericArguments()[0];
                                    }

                                    nowSerializeInfo.Length_FieldInfo = (int)brData.ReadUInt32();
                                }
                                else if (nowSerializeInfo.TheType.GetInterface("IDictionary`2", false) != null)
                                {
                                    nowSerializeInfo.IsIDictionary = true;

                                    object[] ubas = nowSerializeInfo.TheType.GetCustomAttributes(false);
                                    if (ubas.Length > 0)
                                    {
                                        UseBaseArgument uba = ubas[0] as UseBaseArgument;
                                        if (uba != null)
                                        {
                                            nowSerializeInfo.TheElementType = nowSerializeInfo.TheType.BaseType.GetGenericArguments()[0];
                                            nowSerializeInfo.TheValueType = nowSerializeInfo.TheType.BaseType.GetGenericArguments()[1];
                                        }
                                        else
                                        {
                                            nowSerializeInfo.TheElementType = nowSerializeInfo.TheType.GetGenericArguments()[0];
                                            nowSerializeInfo.TheValueType = nowSerializeInfo.TheType.GetGenericArguments()[1];
                                        }
                                    }
                                    else
                                    {
                                        nowSerializeInfo.TheElementType = nowSerializeInfo.TheType.GetGenericArguments()[0];
                                        nowSerializeInfo.TheValueType = nowSerializeInfo.TheType.GetGenericArguments()[1];
                                    }

                                    nowSerializeInfo.Length_FieldInfo = (int)brData.ReadUInt32();

                                    nowSerializeInfo.Keys = new object[nowSerializeInfo.Length_FieldInfo / 2];
                                }
                                else
                                {
                                    DynamicFieldInfo[] fis;


                                    lock (FI_Lookup)
                                    {
                                        if (FI_Lookup.ContainsKey(nowSerializeInfo.TheType))
                                        {
                                            fis = FI_Lookup[nowSerializeInfo.TheType];
                                        }
                                        else
                                        {
                                            PropertyInfo[] fi = nowSerializeInfo.TheType.GetProperties(AllInstanceBindingFlags).OrderBy(c => c.Name).ToArray();
                                            List<DynamicFieldInfo> tempFis = new List<DynamicFieldInfo>();
                                            for (int i = 0; i < fi.Length; i++)
                                            {
                                                PropertyInfo fiNow = fi[i];
                                                object[] at = fiNow.GetCustomAttributes(typeof(System.NonSerializedAttribute), true);
                                                if (at.Length == 0)
                                                {
                                                    tempFis.Add(new DynamicFieldInfo() { TheField = fiNow });
                                                }
                                            }
                                            fis = tempFis.ToArray();
                                            FI_Lookup.Add(nowSerializeInfo.TheType, fis);
                                        }
                                    }


                                    nowSerializeInfo.FieldInfos = fis;
                                    nowSerializeInfo.Length_FieldInfo = fis.Length;
                                }

                                if (!nowSerializeInfo.TheType.IsInterface)
                                {
                                    nowSerializeInfo.CreatObject();
                                }

                                if (fatherObject == null)
                                {
                                    fatherObject = nowSerializeInfo.TheObject;
                                }
                                else
                                {
                                    if (nowSerializeInfo.FatherSerializeInfo.IsIDictionary)
                                    {
                                        if (nowSerializeInfo.FatherSerializeInfo.Index < nowSerializeInfo.FatherSerializeInfo.Length_FieldInfo / 2)
                                        {
                                            //key值
                                            nowSerializeInfo.FatherSerializeInfo.Keys[nowSerializeInfo.FatherSerializeInfo.Index] = nowSerializeInfo.TheObject;
                                        }
                                        else
                                        {
                                            //value值
                                            nowSerializeInfo.FatherSerializeInfo.SetInnerField(nowSerializeInfo.TheObject);
                                        }
                                    }
                                    else
                                    {
                                        nowSerializeInfo.FatherSerializeInfo.SetInnerField(nowSerializeInfo.TheObject);
                                    }
                                    nowSerializeInfo.FatherSerializeInfo.Index++;
                                }

                                if (nowSerializeInfo.Length_FieldInfo != 0)
                                {
                                    if (fullLengthSchema)
                                    {
                                        metablockLen = brData.TryReadMetaBlockLength();
                                        nextSeekTo = brData.Position + metablockLen;

                                        //if (readEnd > 0 && readEnd < nextSeekTo)
                                        //{
                                        //    continue;
                                        //}
                                    }

                                    if (nowSerializeInfo.IsArray || nowSerializeInfo.IsIList)
                                    {
                                        innerSerializeInfo = new SerializeInfo(nowSerializeInfo.TheElementType, nextSeekTo);
                                    }
                                    else if (nowSerializeInfo.IsIDictionary)
                                    {
                                        innerSerializeInfo = new SerializeInfo(nowSerializeInfo.TheElementType, nextSeekTo);
                                    }
                                    else
                                    {
                                        DynamicFieldInfo tempDFI = nowSerializeInfo.FieldInfos[0];
                                        innerSerializeInfo = new SerializeInfo(tempDFI.TheField.PropertyType, nextSeekTo);
                                    }
                                    innerSerializeInfo.FatherSerializeInfo = nowSerializeInfo;
                                    bufStack.Push(innerSerializeInfo);
                                    
                                }
                            }

                        }
                    }
                    else  //检查过
                    {
                        if (nowSerializeInfo.Length_FieldInfo == nowSerializeInfo.Index)  //检查完了
                        {
                            bufStack.Pop();
                            if (fullLengthSchema)
                            {
                                if (readEnd >= totalLength)
                                {
                                    break;
                                }
                                if (readEnd > 0 && readEnd > brData.Position)
                                {
                                    brData.SeekNextTo(readEnd);
                                }
                            }
                        }
                        else
                        {
                            if (fullLengthSchema)
                            {
                                metablockLen = brData.TryReadMetaBlockLength();
                                nextSeekTo = brData.Position + metablockLen;

                                if ((readEnd > 0 && readEnd < nextSeekTo))
                                {
                                    nowSerializeInfo.Index = nowSerializeInfo.Length_FieldInfo;
                                    brData.SeekAdd(-2);
                                    continue;
                                }
                            }

                            if (nowSerializeInfo.IsArray || nowSerializeInfo.IsIList)
                            {
                                innerSerializeInfo = new SerializeInfo(nowSerializeInfo.TheElementType, nextSeekTo);
                            }
                            else if (nowSerializeInfo.IsIDictionary)
                            {
                                if (nowSerializeInfo.Index < nowSerializeInfo.Length_FieldInfo / 2)
                                {
                                    innerSerializeInfo = new SerializeInfo(nowSerializeInfo.TheElementType, nextSeekTo);
                                }
                                else
                                {
                                    innerSerializeInfo = new SerializeInfo(nowSerializeInfo.TheValueType, nextSeekTo);
                                }
                            }
                            else
                            {
                                DynamicFieldInfo tempDFI = nowSerializeInfo.FieldInfos[nowSerializeInfo.Index];
                                innerSerializeInfo = new SerializeInfo(tempDFI.TheField.PropertyType, nextSeekTo);
                            }
                            innerSerializeInfo.FatherSerializeInfo = nowSerializeInfo;
                            bufStack.Push(innerSerializeInfo);
                        }
                    }
                }

                if (fullLengthSchema)
                {
                    readLength = totalLength;
                }
                else
                {
                    readLength = brData.Position;
                }
                return fatherObject;
            }
            catch (SerializeException se)
            {
                throw new SerializeException(objectType.ToString() + se.Message);
            }
            //catch (Exception e)
            {
                //throw new SerializeException(objectType.ToString() + "Data bytes and type not match" + e.Message);
            }
        }


        public static object DeserializeFrom(Type objectType, byte[] bytesValue, int startIndex, out int readLength)
        {
            return DeserializeFrom(objectType, bytesValue, startIndex, Int32.MaxValue / 2, out readLength);
        }


        /// <summary>
        /// 反序列化Byte数组到一个对象，无法反序列化，将抛出ArgumentException异常
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="bytesValue">二进制数据</param>
        /// <param name="startIndex">开始执行的位置</param>
        /// <param name="maxReadSize">指定支持最大反序列化的数据大小</param>
        /// <param name="readLength">实际反序列化时读取的元素个数</param>
        /// <returns>对象</returns>
        public static T DeserializeFrom<T>(byte[] bytesValue, int startIndex, int maxReadSize, out int readLength)
        {
            return (T)DeserializeFrom(typeof(T), bytesValue, startIndex, maxReadSize, out readLength);
        }


        public static T DeserializeFrom<T>(byte[] bytesValue, int startIndex, out int readLength)
        {
            return (T)DeserializeFrom(typeof(T), bytesValue, startIndex, Int32.MaxValue / 2, out readLength);
        }
    }



    internal class SerializeInfo
    {
        private static Type[] Type_Zero = new Type[0];

        internal static Type Type_Nullable = typeof(Nullable<>);


        public object TheObject;

        //给字典表存放key准备
        public object[] Keys;

        public Type TheType;

        public DynamicFieldInfo[] FieldInfos;

        public Type TheValueType;

        public Type TheElementType;

        public bool IsArray;

        public bool IsIList;

        public bool IsIDictionary;

        public bool IsNullable;

        public int Length_FieldInfo;

        public int Index;

        public SerializeInfo FatherSerializeInfo;

        public int ByteStartIndexOrEndPoint;

        /// <summary>
        /// 序列化时使用
        /// </summary>
        /// <param name="theObject"></param>
        /// <param name="isNullable"></param>
        /// <param name="byteStartIndex"></param>
        public SerializeInfo(object theObject, bool isNullable, int byteStartIndex)
        {
            TheObject = theObject;
            IsNullable = isNullable;
            ByteStartIndexOrEndPoint = byteStartIndex;
        }

        /// <summary>
        /// 反序列化时使用
        /// </summary>
        /// <param name="theType"></param>
        /// <param name="byteEndPoint"></param>
        public SerializeInfo(Type theType, int byteEndPoint)
        {
            TheType = theType;
            ByteStartIndexOrEndPoint = byteEndPoint;
        }

        public object GetInnerField(ref bool isFieldNullable)
        {
            if (IsArray)
            {
                return (TheObject as Array).GetValue(Index);
            }
            else if (IsIList)
            {
                return (TheObject as IList)[Index];
            }
            else if (IsIDictionary)
            {
                return (TheObject as Array).GetValue(Index);
            }
            else
            {
                Type fieldType = FieldInfos[Index].TheField.PropertyType;

                //专门处理Nullable类型
                if (fieldType.IsGenericType)
                {
                    if (fieldType.GetGenericTypeDefinition() == Type_Nullable)
                    {
                        isFieldNullable = true;
                    }
                }

                return FieldInfos[Index].TheField.GetValue(TheObject);
            }
        }

        public void SetInnerField(object innerObject, bool innerCall = false)
        {
            if (IsArray)
            {
                (TheObject as Array).SetValue(innerObject, Index);
            }
            else if (IsIList)
            {
                if (innerCall)
                {
                    (TheObject as IList).RemoveAt(Index);
                }
                (TheObject as IList).Add(innerObject);
            }
            else if (IsIDictionary)
            {
                if (innerCall)
                {
                    (TheObject as IDictionary).Remove(Keys[Index - Length_FieldInfo / 2]);
                }
                (TheObject as IDictionary).Add(Keys[Index - Length_FieldInfo / 2], innerObject);
            }
            else
            {
                if (TheType.IsClass)
                {
                    FieldInfos[Index].TheField.SetValue(TheObject, innerObject);
                }
                else
                {

                    FieldInfos[Index].TheField.SetValue(TheObject, innerObject);
                    if (FatherSerializeInfo != null && Index == Length_FieldInfo - 1)
                    {
                        FatherSerializeInfo.Index--;
                        FatherSerializeInfo.SetInnerField(TheObject, true);
                        FatherSerializeInfo.Index++;
                        //FatherSerializeInfo.FieldInfos[FatherSerializeInfo.Index - 1].TheField.SetValue(FatherSerializeInfo.TheObject, TheObject);
                    }

                }
            }
        }


        public void CreatObject()
        {
            if (IsArray)
            {
                TheObject = Array.CreateInstance(TheType.GetElementType(), Length_FieldInfo);
            }
            else
            {
                TheObject = Activator.CreateInstance(TheType);
            }
        }
    }


    /// <summary>
    /// 表示是否使用基类的泛型参数信息
    /// </summary>
    public class UseBaseArgument : System.Attribute
    {
        public UseBaseArgument()
        {
        }
    }
}
