using System;

namespace GN
{
    /// <summary>
    /// 从一串byte数组中获取基础值类型（和对应数组）
    /// </summary>
    public class BytesReader
    {
        private const byte BYTE_ZERO = 0;


        /// <summary>
        /// 当前Bytes的index
        /// </summary>
        private int PointAt;


        /// <summary>
        /// 允许读取的最大长度
        /// </summary>
        private int MaxSize;

        /// <summary>
        /// 获取当前下一个读取的位置
        /// </summary>
        public int Position
        {
            get { return PointAt; }
        }

        /// <summary>
        /// 读取值
        /// </summary>
        private byte[] Bytes;


        

        #region 缓存
        byte[] byteBuf = new byte[16];

        int[] intBuf = new int[4];
        #endregion



        /// <summary>
        /// 初始化一个BytesReader
        /// </summary>
        /// <param name="startPoint">起始读取位置</param>
        /// <param name="maxSize">允许在当前byte[]中所读取的最大长度 ,从0开始算的</param>
        /// <param name="bytes">读取的byte数组</param>
        public BytesReader(int startPoint, int maxSize, byte[] bytes)
        {
            if (startPoint >= maxSize)
            {
                throw new ArgumentOutOfRangeException("startPoint must < maxSize");
            }

            PointAt = startPoint;
            MaxSize = maxSize;
            Bytes = bytes;
        }


        /// <summary>
        /// 动态增加可读取的大小
        /// </summary>
        /// <param name="addValue">增加的值</param>
        /// <returns>返回新值</returns>
        public int AddMaxSize(int addValue)
        {
            int newValue = MaxSize + addValue;
            if (newValue < MaxSize)
            {
                throw new ArgumentException("addValue must >= 0");
            }
            MaxSize = newValue;
            return newValue;
        }


        public bool ReadBoolean()
        {
            int pointAt = PointAt;
            if (pointAt >= MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            bool result = false;

            if (Bytes[pointAt] != BYTE_ZERO)
            {
                result = true;
            }
            PointAt = pointAt + 1;

            return result;
        }


        public byte ReadByte()
        {
            int pointAt = PointAt;
            if (pointAt >= MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            byte result = Bytes[pointAt];
            PointAt = pointAt + 1;

            return result;
        }


        public sbyte ReadSByte()
        {
            int pointAt = PointAt;
            if (pointAt >= MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            sbyte result = (sbyte)Bytes[pointAt];
            PointAt = pointAt + 1;

            return result;
        }


        public UInt16 ReadUInt16()
        {
            int pointAt = PointAt;
            if (pointAt + 2 > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            UInt16 result = (UInt16)(Bytes[pointAt] | Bytes[pointAt + 1] << 8);
            PointAt = pointAt + 2;

            return result;
        }


        public Int16 ReadInt16()
        {
            int pointAt = PointAt;
            if (pointAt + 2 > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            Int16 result = (Int16)(Bytes[pointAt] | Bytes[pointAt + 1] << 8);
            PointAt = pointAt + 2;

            return result;
        }


        public Char ReadChar()
        {
            int pointAt = PointAt;
            if (pointAt + 2 > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            Char result = (Char)(Bytes[pointAt] | Bytes[pointAt + 1] << 8);
            PointAt = pointAt + 2;

            return result;
        }


        public UInt32 ReadUInt32()
        {
            int pointAt = PointAt;
            if (pointAt + 4 > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            UInt32 result = 0;

            result = (UInt32)(Bytes[pointAt] | Bytes[pointAt + 1] << 8 | Bytes[pointAt + 2] << 16 | Bytes[pointAt + 3] << 24);
            PointAt = pointAt + 4;

            return result;
        }



        public Int32 ReadInt32()
        {
            int pointAt = PointAt;
            if (pointAt + 4 > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            Int32 result = 0;

            result = Bytes[pointAt] | Bytes[pointAt + 1] << 8 | Bytes[pointAt + 2] << 16 | Bytes[pointAt + 3] << 24;

            PointAt = pointAt + 4;

            return result;
        }



        public UInt64 ReadUInt64()
        {
            int pointAt = PointAt;
            if (pointAt + 8 > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            UInt64 result = 0;

            result = (UInt64)Bytes[pointAt] | (UInt64)Bytes[pointAt + 1] << 8 | (UInt64)Bytes[pointAt + 2] << 16 | (UInt64)Bytes[pointAt + 3] << 24 | (UInt64)Bytes[pointAt + 4] << 32 | (UInt64)Bytes[pointAt + 5] << 40 | (UInt64)Bytes[pointAt + 6] << 48 | (UInt64)Bytes[pointAt + 7] << 56;

            PointAt = pointAt + 8;

            return result;
        }


        public Int64 ReadInt64()
        {
            int pointAt = PointAt;
            if (pointAt + 8 > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            Int64 result = 0;
            result = (Int64)Bytes[pointAt] | (Int64)Bytes[pointAt + 1] << 8 | (Int64)Bytes[pointAt + 2] << 16 | (Int64)Bytes[pointAt + 3] << 24 | (Int64)Bytes[pointAt + 4] << 32 | (Int64)Bytes[pointAt + 5] << 40 | (Int64)Bytes[pointAt + 6] << 48 | (Int64)Bytes[pointAt + 7] << 56;
            
            PointAt = pointAt + 8;

            return result;
        }



        public Single ReadSingle()
        {
            int pointAt = PointAt;
            if (pointAt + 4 > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            Single result = 0;

            result = BitConverter.ToSingle(Bytes, pointAt);

            PointAt = pointAt + 4;

            return result;
        }




        public Double ReadDouble()
        {
            int pointAt = PointAt;
            if (pointAt + 8 > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            Double result = 0;

            result = BitConverter.ToDouble(Bytes, pointAt);

            PointAt = pointAt + 8;

            return result;
        }




        public Decimal ReadDecimal()
        {
            int pointAt = PointAt;
            if (pointAt + 16 > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            Decimal result = 0;

            intBuf[0] = BitConverter.ToInt32(Bytes, pointAt + 8);
            intBuf[1] = BitConverter.ToInt32(Bytes, pointAt + 12);
            intBuf[2] = BitConverter.ToInt32(Bytes, pointAt + 4);
            intBuf[3] = BitConverter.ToInt32(Bytes, pointAt);
            result = new decimal(intBuf);

            PointAt = pointAt + 16;

            return result;
        }




        public Guid ReadGuid()
        {
            int pointAt = PointAt;
            if (pointAt + 16 > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            Guid result = new Guid();

            Buffer.BlockCopy(Bytes, pointAt, byteBuf, 0, 16);

            result = new Guid(byteBuf);

            PointAt = pointAt + 16;

            return result;
        }



        public DateTime ReadDateTime()
        {
            int pointAt = PointAt;
            if (pointAt + 8 > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }


            Buffer.BlockCopy(Bytes, pointAt, byteBuf, 0, 8);

            Int64 dateData = BitConverter.ToInt64(byteBuf, 0);

            DateTime result = new DateTime(dateData & 0x3fffffffffffffffL, (DateTimeKind)((ulong)dateData >> 0x3e));

            PointAt = pointAt + 8;
            return result;
        }



        public TimeSpan ReadTimeSpan()
        {
            int pointAt = PointAt;
            if (pointAt + 8 > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }


            Buffer.BlockCopy(Bytes, pointAt, byteBuf, 0, 8);
            TimeSpan result = new TimeSpan(BitConverter.ToInt64(byteBuf, 0));


            PointAt = pointAt + 8;
            return result;
        }



        public bool[] ReadBooleans()
        {
            byte isNull = ReadByte();
            if (isNull == BYTE_ZERO)
            {
                return null;
            }

            int byteLength = ReadInt32();

            int pointAt = PointAt;
            if (pointAt + byteLength > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            bool[] result = new bool[byteLength];

            Buffer.BlockCopy(Bytes, pointAt, result, 0, byteLength);

            PointAt = pointAt + byteLength;

            return result;
        }



        public byte[] ReadBytes()
        {
            byte isNull = ReadByte();
            if (isNull == BYTE_ZERO)
            {
                return null;
            }

            int byteLength = ReadInt32();

            int pointAt = PointAt;
            if (pointAt + byteLength > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            byte[] result = new byte[byteLength];

            Buffer.BlockCopy(Bytes, pointAt, result, 0, byteLength);

            PointAt = pointAt + byteLength;

            return result;
        }



        public sbyte[] ReadSBytes()
        {
            byte isNull = ReadByte();
            if (isNull == BYTE_ZERO)
            {
                return null;
            }

            int byteLength = ReadInt32();

            int pointAt = PointAt;
            if (pointAt + byteLength > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            sbyte[] result = new sbyte[byteLength];

            Buffer.BlockCopy(Bytes, pointAt, result, 0, byteLength);

            PointAt = pointAt + byteLength;

            return result;
        }



        public UInt16[] ReadUInt16s()
        {
            byte isNull = ReadByte();
            if (isNull == BYTE_ZERO)
            {
                return null;
            }

            int byteLength = ReadInt32();

            int pointAt = PointAt;
            if (pointAt + byteLength > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            UInt16[] result = new UInt16[byteLength >> 1];

            Buffer.BlockCopy(Bytes, pointAt, result, 0, byteLength);
            PointAt = pointAt + byteLength;

            return result;
        }



        public Int16[] ReadInt16s()
        {
            byte isNull = ReadByte();
            if (isNull == BYTE_ZERO)
            {
                return null;
            }

            int byteLength = ReadInt32();

            int pointAt = PointAt;
            if (pointAt + byteLength > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            Int16[] result = new Int16[byteLength >> 1];

            Buffer.BlockCopy(Bytes, pointAt, result, 0, byteLength);

            PointAt = pointAt + byteLength;

            return result;
        }




        public Char[] ReadChars()
        {
            byte isNull = ReadByte();
            if (isNull == BYTE_ZERO)
            {
                return null;
            }

            int byteLength = ReadInt32();

            int pointAt = PointAt;
            if (pointAt + byteLength > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            Char[] result = new Char[byteLength >> 1];

            Buffer.BlockCopy(Bytes, pointAt, result, 0, byteLength);

            PointAt = pointAt + byteLength;

            return result;
        }





        public UInt32[] ReadUInt32s()
        {
            byte isNull = ReadByte();
            if (isNull == BYTE_ZERO)
            {
                return null;
            }

            int byteLength = ReadInt32();

            int pointAt = PointAt;
            if (pointAt + byteLength > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            UInt32[] result = new UInt32[byteLength >> 2];

            Buffer.BlockCopy(Bytes, pointAt, result, 0, byteLength);

            PointAt = pointAt + byteLength;

            return result;
        }





        public Int32[] ReadInt32s()
        {
            byte isNull = ReadByte();
            if (isNull == BYTE_ZERO)
            {
                return null;
            }

            int byteLength = ReadInt32();

            int pointAt = PointAt;
            if (pointAt + byteLength > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            Int32[] result = new Int32[byteLength >> 2];

            Buffer.BlockCopy(Bytes, pointAt, result, 0, byteLength);

            PointAt = pointAt + byteLength;

            return result;
        }




        public UInt64[] ReadUInt64s()
        {
            byte isNull = ReadByte();
            if (isNull == BYTE_ZERO)
            {
                return null;
            }

            int byteLength = ReadInt32();

            int pointAt = PointAt;
            if (pointAt + byteLength > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            UInt64[] result = new UInt64[byteLength >> 3];

            Buffer.BlockCopy(Bytes, pointAt, result, 0, byteLength);

            PointAt = pointAt + byteLength;

            return result;
        }






        public Int64[] ReadInt64s()
        {
            byte isNull = ReadByte();
            if (isNull == BYTE_ZERO)
            {
                return null;
            }

            int byteLength = ReadInt32();

            int pointAt = PointAt;
            if (pointAt + byteLength > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            Int64[] result = new Int64[byteLength >> 3];

            Buffer.BlockCopy(Bytes, pointAt, result, 0, byteLength);

            PointAt = pointAt + byteLength;

            return result;
        }



        public Single[] ReadSingles()
        {
            byte isNull = ReadByte();
            if (isNull == BYTE_ZERO)
            {
                return null;
            }

            int byteLength = ReadInt32();

            int pointAt = PointAt;
            if (pointAt + byteLength > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            Single[] result = new Single[byteLength >> 2];

            Buffer.BlockCopy(Bytes, pointAt, result, 0, byteLength);

            PointAt = pointAt + byteLength;

            return result;
        }




        public Double[] ReadDoubles()
        {
            byte isNull = ReadByte();
            if (isNull == BYTE_ZERO)
            {
                return null;
            }

            int byteLength = ReadInt32();

            int pointAt = PointAt;
            if (pointAt + byteLength > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            Double[] result = new Double[byteLength >> 3];

            Buffer.BlockCopy(Bytes, pointAt, result, 0, byteLength);

            PointAt = pointAt + byteLength;

            return result;
        }




        public Decimal[] ReadDecimals()
        {
            byte isNull = ReadByte();
            if (isNull == BYTE_ZERO)
            {
                return null;
            }

            int byteLength = ReadInt32();

            int pointAt = PointAt;
            if (pointAt + byteLength > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            int length = byteLength >> 4;
            Decimal[] result = new Decimal[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = ReadDecimal();
            }
            
            return result;
        }



        public Guid[] ReadGuids()
        {
            byte isNull = ReadByte();
            if (isNull == BYTE_ZERO)
            {
                return null;
            }

            int byteLength = ReadInt32();

            int pointAt = PointAt;
            if (pointAt + byteLength > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            int length = byteLength >> 4;
            Guid[] result = new Guid[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = ReadGuid();
            }


            return result;
        }




        public DateTime[] ReadDateTimes()
        {
            byte isNull = ReadByte();
            if (isNull == BYTE_ZERO)
            {
                return null;
            }

            int byteLength = ReadInt32();

            int pointAt = PointAt;
            if (pointAt + byteLength > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            int length = byteLength >> 3;
            DateTime[] result = new DateTime[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = ReadDateTime();
            }


            return result;
        }




        public TimeSpan[] ReadTimeSpans()
        {
            byte isNull = ReadByte();
            if (isNull == BYTE_ZERO)
            {
                return null;
            }

            int byteLength = ReadInt32();

            int pointAt = PointAt;
            if (pointAt + byteLength > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            int length = byteLength >> 3;
            TimeSpan[] result = new TimeSpan[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = ReadTimeSpan();
            }

            return result;
        }




        public string ReadString()
        {
            byte isNull = ReadByte();
            if (isNull == BYTE_ZERO)
            {
                return null;
            }

            int byteLength = ReadInt32();

            int pointAt = PointAt;
            if (pointAt + byteLength > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            char[] result = new char[byteLength >> 1];

            Buffer.BlockCopy(Bytes, pointAt, result, 0, byteLength);

            PointAt = pointAt + byteLength;

            return new string(result);
        }


        public static Guid ConverToGuid(byte[] bytes, int index)
        {
            V128 v = default(V128);

            v.B00 = bytes[index];
            v.B01 = bytes[index + 1];
            v.B02 = bytes[index + 2];
            v.B03 = bytes[index + 3];
            v.B04 = bytes[index + 4];
            v.B05 = bytes[index + 5];
            v.B06 = bytes[index + 6];
            v.B07 = bytes[index + 7];
            v.B08 = bytes[index + 8];
            v.B09 = bytes[index + 9];
            v.B10 = bytes[index + 10];
            v.B11 = bytes[index + 11];
            v.B12 = bytes[index + 12];
            v.B13 = bytes[index + 13];
            v.B14 = bytes[index + 14];
            v.B15 = bytes[index + 15];

            return v.G;
        }
    }
}
