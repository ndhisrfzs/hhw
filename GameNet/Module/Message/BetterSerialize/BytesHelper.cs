using System;
using System.Reflection;

namespace GN
{
    /// <summary>
    /// 尝试直接序列化和反序列化基础值类型
    /// </summary>
    public static class BytesHelper
    {
        public static Type Type_Bool = typeof(bool);
        public static Type Type_Byte = typeof(byte);
        public static Type Type_SByte = typeof(sbyte);
        public static Type Type_UInt16 = typeof(UInt16);
        public static Type Type_Int16 = typeof(Int16);
        public static Type Type_Char = typeof(Char);
        public static Type Type_UInt32 = typeof(UInt32);
        public static Type Type_Int32 = typeof(Int32);
        public static Type Type_UInt64 = typeof(UInt64);
        public static Type Type_Int64 = typeof(Int64);
        public static Type Type_Single = typeof(Single);
        public static Type Type_Double = typeof(Double);
        public static Type Type_Decimal = typeof(Decimal);
        public static Type Type_Guid = typeof(Guid);
        public static Type Type_DateTime = typeof(DateTime);
        public static Type Type_TimeSpan = typeof(TimeSpan);

        public static Type Type_Bools = typeof(bool[]);
        public static Type Type_Bytes = typeof(byte[]);
        public static Type Type_SBytes = typeof(sbyte[]);
        public static Type Type_UInt16s = typeof(UInt16[]);
        public static Type Type_Int16s = typeof(Int16[]);
        public static Type Type_Chars = typeof(Char[]);
        public static Type Type_UInt32s = typeof(UInt32[]);
        public static Type Type_Int32s = typeof(Int32[]);
        public static Type Type_UInt64s = typeof(UInt64[]);
        public static Type Type_Int64s = typeof(Int64[]);
        public static Type Type_Singles = typeof(Single[]);
        public static Type Type_Doubles = typeof(Double[]);
        public static Type Type_Decimals = typeof(Decimal[]);
        public static Type Type_Guids = typeof(Guid[]);
        public static Type Type_DateTimes = typeof(DateTime[]);
        public static Type Type_TimeSpans = typeof(TimeSpan[]);

        public static Type Type_String = typeof(string);


        /// <summary>
        /// 尝试直接序列化基础值类型（包括其数组）
        /// </summary>
        /// <param name="value">对象</param>
        /// <param name="type">对象的类型</param>
        /// <param name="theByteWriter">写入Write</param>
        /// <returns>能否执行</returns>
        public static bool TryWriteBytes(object value, Type type, ZipBytesWriter theByteWriter)
        {
            if (value == null)
            {
                return false;
            }
            
            if (type.IsEnum)
            {
                type = type.GetFields()[0].FieldType;
            }

            if (type == Type_Bool)
            {
                theByteWriter.Write((bool)value);
                return true;
            }
            if (type == Type_Byte)
            {
                theByteWriter.Write((byte)value);
                return true;
            }
            if (type == Type_SByte)
            {
                theByteWriter.Write((sbyte)value);
                return true;
            }
            if (type == Type_UInt16)
            {
                theByteWriter.Write((UInt16)value);
                return true;
            }
            if (type == Type_Int16)
            {
                theByteWriter.Write((Int16)value);
                return true;
            }
            if (type == Type_Char)
            {
                theByteWriter.Write((Char)value);
                return true;
            }
            if (type == Type_UInt32)
            {
                theByteWriter.Write((UInt32)value);
                return true;
            }
            if (type == Type_Int32)
            {
                theByteWriter.Write((Int32)value);
                return true;
            }
            if (type == Type_UInt64)
            {
                theByteWriter.Write((UInt64)value);
                return true;
            }
            if (type == Type_Int64)
            {
                theByteWriter.Write((Int64)value);
                return true;
            }
            if (type == Type_Single)
            {
                theByteWriter.Write((Single)value);
                return true;
            }
            if (type == Type_Double)
            {
                theByteWriter.Write((Double)value);
                return true;
            }
            if (type == Type_Decimal)
            {
                theByteWriter.Write((Decimal)value);
                return true;
            }
            if (type == Type_Guid)
            {
                theByteWriter.Write((Guid)value);
                return true;
            }
            if (type == Type_DateTime)
            {
                theByteWriter.Write((DateTime)value);
                return true;
            }
            if (type == Type_TimeSpan)
            {
                theByteWriter.Write((TimeSpan)value);
                return true;
            }
            if (type == Type_String)
            {
                theByteWriter.Write((String)value);
                return true;
            }

            
            if (type.IsArray)
            {
                Type elemtT = type.GetElementType();
                if (elemtT.IsEnum)
                {
#if Server || WP
                    type = elemtT.GetFields()[0].FieldType;
                    type = type.MakeArrayType();

#else
                    Type elemtUnderType = null;
                    FieldInfo elemtValueField = null;

                    elemtValueField = elemtT.GetFields()[0];
                    elemtUnderType = elemtValueField.FieldType;
                    type = elemtUnderType.MakeArrayType();

                    Array oArray = value as Array;
                    int arrayLength = oArray.Length;
                    Array newArray = Array.CreateInstance(elemtUnderType, arrayLength);
                    Array.Copy(oArray, 0, newArray, 0, arrayLength);

                    value = newArray;
#endif
                }
            }

            if (type == Type_Bools)
            {
                theByteWriter.Write((bool[])value);
                return true;
            }
            if (type == Type_Bytes)
            {
                theByteWriter.Write((byte[])value, ((byte[])value).Length);
                return true;
            }
            if (type == Type_SBytes)
            {
                theByteWriter.Write((sbyte[])value);
                return true;
            }
            if (type == Type_UInt16s)
            {
                theByteWriter.Write((UInt16[])value);
                return true;
            }
            if (type == Type_Int16s)
            {
                theByteWriter.Write((Int16[])value);
                return true;
            }
            if (type == Type_Chars)
            {
                theByteWriter.Write((Char[])value);
                return true;
            }
            if (type == Type_UInt32s)
            {
                theByteWriter.Write((UInt32[])value);
                return true;
            }
            if (type == Type_Int32s)
            {
                theByteWriter.Write((Int32[])value);
                return true;
            }
            if (type == Type_UInt64s)
            {
                theByteWriter.Write((UInt64[])value);
                return true;
            }
            if (type == Type_Int64s)
            {
                theByteWriter.Write((Int64[])value);
                return true;
            }
            if (type == Type_Singles)
            {
                theByteWriter.Write((Single[])value);
                return true;
            }
            if (type == Type_Doubles)
            {
                theByteWriter.Write((Double[])value);
                return true;
            }
            if (type == Type_Decimals)
            {
                theByteWriter.Write((Decimal[])value);
                return true;
            }
            if (type == Type_Guids)
            {
                theByteWriter.Write((Guid[])value);
                return true;
            }
            if (type == Type_DateTimes)
            {
                theByteWriter.Write((DateTime[])value);
                return true;
            }
            if (type == Type_TimeSpans)
            {
                theByteWriter.Write((TimeSpan[])value);
                return true;
            }

            return false;
        }


        /// <summary>
        /// 尝试直接反序列化基础值类型（包括其数组）
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="value">返回的对象</param>
        /// <param name="theByteReader">读取reader</param>
        /// <returns>能否执行</returns>
        public static bool TryGetValue(Type type, out object value, ZipBytesReader theByteReader)
        {
            value = null;

            if (type.IsEnum)
            {
                type = type.GetFields()[0].FieldType;
            }

            if (type == Type_Bool)
            {
                value = theByteReader.ReadBoolean();
                return true;
            }
            if (type == Type_Byte)
            {
                value = theByteReader.ReadByte();
                return true;
            }
            if (type == Type_SByte)
            {
                value = theByteReader.ReadSByte();
                return true;
            }
            if (type == Type_UInt16)
            {
                value = theByteReader.ReadUInt16();
                return true;
            }
            if (type == Type_Int16)
            {
                value = theByteReader.ReadInt16();
                return true;
            }
            if (type == Type_Char)
            {
                value = theByteReader.ReadChar();
                return true;
            }
            if (type == Type_UInt32)
            {
                value = theByteReader.ReadUInt32();
                return true;
            }
            if (type == Type_Int32)
            {
                value = theByteReader.ReadInt32();
                return true;
            }
            if (type == Type_UInt64)
            {
                value = theByteReader.ReadUInt64();
                return true;
            }
            if (type == Type_Int64)
            {
                value = theByteReader.ReadInt64();
                return true;
            }
            if (type == Type_Single)
            {
                value = theByteReader.ReadSingle();
                return true;
            }
            if (type == Type_Double)
            {
                value = theByteReader.ReadDouble();
                return true;
            }
            if (type == Type_Decimal)
            {
                value = theByteReader.ReadDecimal();
                return true;
            }
            if (type == Type_Guid)
            {
                value = theByteReader.ReadGuid();
                return true;
            }
            if (type == Type_DateTime)
            {
                value = theByteReader.ReadDateTime();
                return true;
            }
            if (type == Type_TimeSpan)
            {
                value = theByteReader.ReadTimeSpan();
                return true;
            }
            if (type == Type_String)
            {
                value = theByteReader.ReadString();
                return true;
            }

            bool isEnumArray = false;
            Type elemtT = null;
            FieldInfo elemtValueField = null;
            if (type.IsArray)
            {
                elemtT = type.GetElementType();
                if (elemtT.IsEnum)
                {
                    isEnumArray = true;

                    elemtValueField = elemtT.GetFields()[0];

                    type = elemtValueField.FieldType.MakeArrayType();
                }
            }

            if (type == Type_Bools)
            {
                value = theByteReader.ReadBooleans();
                goto GoTrueEnd;
            }
            if (type == Type_Bytes)
            {
                value = theByteReader.ReadBytes();
                goto GoTrueEnd;
            }
            if (type == Type_SBytes)
            {
                value = theByteReader.ReadSBytes();
                goto GoTrueEnd;
            }
            if (type == Type_UInt16s)
            {
                value = theByteReader.ReadUInt16s();
                goto GoTrueEnd;
            }
            if (type == Type_Int16s)
            {
                value = theByteReader.ReadInt16s();
                goto GoTrueEnd;
            }
            if (type == Type_Chars)
            {
                value = theByteReader.ReadChars();
                goto GoTrueEnd;
            }
            if (type == Type_UInt32s)
            {
                value = theByteReader.ReadUInt32s();
                goto GoTrueEnd;
            }
            if (type == Type_Int32s)
            {
                value = theByteReader.ReadInt32s();
                goto GoTrueEnd;
            }
            if (type == Type_UInt64s)
            {
                value = theByteReader.ReadUInt64s();
                goto GoTrueEnd;
            }
            if (type == Type_Int64s)
            {
                value = theByteReader.ReadInt64s();
                goto GoTrueEnd;
            }
            if (type == Type_Singles)
            {
                value = theByteReader.ReadSingles();
                goto GoTrueEnd;
            }
            if (type == Type_Doubles)
            {
                value = theByteReader.ReadDoubles();
                goto GoTrueEnd;
            }
            if (type == Type_Decimals)
            {
                value = theByteReader.ReadDecimals();
                goto GoTrueEnd;
            }
            if (type == Type_Guids)
            {
                value = theByteReader.ReadGuids();
                goto GoTrueEnd;
            }
            if (type == Type_DateTimes)
            {
                value = theByteReader.ReadDateTimes();
                goto GoTrueEnd;
            }
            if (type == Type_TimeSpans)
            {
                value = theByteReader.ReadTimeSpans();
                goto GoTrueEnd;
            }

            return false;

        GoTrueEnd:
            if (isEnumArray && value != null)
            {
                Array oArray = value as Array;
                int arrayLength = oArray.Length;
                
                Array newArray = Array.CreateInstance(elemtT, arrayLength);
                
                for (int i = 0; i < arrayLength; i++)
                {
                    newArray.SetValue(Enum.ToObject(elemtT, oArray.GetValue(i)), i);
                }

                value = newArray;
            }
            return true;
        }
    }
}
