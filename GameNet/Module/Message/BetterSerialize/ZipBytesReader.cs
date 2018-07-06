using System;

namespace GN
{
    public class ZipBytesReader
    {
        internal const byte BYTE_ZERO = 0;

        internal const byte BYTE_ONE = 0x1;

        /// <summary>
        /// 当前Bytes的index
        /// </summary>
        private int PointAt;


        /// <summary>
        /// 允许读取的最大长度
        /// </summary>
        public int MaxSize;

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



        /// <summary>
        /// 初始化一个BytesReader
        /// </summary>
        /// <param name="startPoint">起始读取位置</param>
        /// <param name="maxSize">允许在当前byte[]中所读取的最大长度 ,从0开始算的</param>
        /// <param name="bytes">读取的byte数组</param>
        public ZipBytesReader(int startPoint, int maxSize, byte[] bytes)
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

        public void SeekNextTo(int newPoint)
        {
            PointAt = newPoint;
        }
        public void SeekAdd(int addPoint)
        {
            PointAt += addPoint;
        }

        public uint ReadRawUInt()
        {
            int pointAt = PointAt;
            if (pointAt + 4 > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            UInt32 result = (UInt32)(Bytes[pointAt] << 24 | Bytes[pointAt + 1] << 16 | Bytes[pointAt + 2] << 8 | Bytes[pointAt + 3]);
            PointAt = pointAt + 4;

            return result;
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

        public ushort TryReadMetaBlockLength()
        {
            int pointAt = PointAt;
            if (pointAt + 2 > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            if (pointAt < Bytes.Length - 1)
            {
                UInt16 result = (UInt16)(Bytes[pointAt] << 8 | Bytes[pointAt + 1]);
                PointAt = pointAt + 2;

                return result;
            }
            else
            {
                return 0;
            }
        }

        public ushort ReadUInt16()
        {
            int pointAt = PointAt;
            if (pointAt + 2 > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            UInt16 result = (UInt16)(Bytes[pointAt] << 8 | Bytes[pointAt + 1]);
            PointAt = pointAt + 2;

            return result;
        }

        public short ReadInt16()
        {
            int pointAt = PointAt;
            if (pointAt + 2 > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            Int16 result = (Int16)(Bytes[pointAt] << 8 | Bytes[pointAt + 1]);
            PointAt = pointAt + 2;

            return result;
        }

        public char ReadChar()
        {
            return (Char)ReadUInt16();
        }


        public uint ReadUInt32()
        {
            int pointAt = PointAt;

            byte firstV = Bytes[pointAt];
            uint resultV = 0;
            if (firstV >= ZipBytesWriter.FLAG_U32_5_111)
            {
                if (pointAt + 5 > MaxSize)
                {
                    throw SerializeException.OverSizeException;
                }

                PointAt = pointAt + 5;

                resultV = ((uint)Bytes[pointAt + 1] << 24 | (uint)Bytes[pointAt + 2] << 16 | (uint)Bytes[pointAt + 3] << 8 | (uint)Bytes[pointAt + 4]);
                //resultV += ZipBytesWriter.MAX_U32_4;

                return resultV;
            }
            else if (firstV >= ZipBytesWriter.FLAG_U32_4_110)
            {
                if (pointAt + 4 > MaxSize)
                {
                    throw SerializeException.OverSizeException;
                }

                PointAt = pointAt + 4;

                resultV = (((uint)firstV & 31) << 24  | (uint)Bytes[pointAt + 1] << 16 | (uint)Bytes[pointAt + 2] << 8 | (uint)Bytes[pointAt + 3]);
                resultV += ZipBytesWriter.MAX_U32_3;

                return resultV;
            }
            else if (firstV >= ZipBytesWriter.FLAG_U32_3_10)
            {
                if (pointAt + 3 > MaxSize)
                {
                    throw SerializeException.OverSizeException;
                }

                PointAt = pointAt + 3;

                resultV = (((uint)firstV & 63) << 16 | (uint)Bytes[pointAt + 1] << 8 | (uint)Bytes[pointAt + 2]);
                resultV += ZipBytesWriter.MAX_U32_2;

                return resultV;
            }
            else if (firstV >= ZipBytesWriter.FLAG_U32_2_01)
            {
                if (pointAt + 2 > MaxSize)
                {
                    throw SerializeException.OverSizeException;
                }

                PointAt = pointAt + 2;

                resultV = (((uint)firstV & 63) << 8 | (uint)Bytes[pointAt + 1]);
                resultV += ZipBytesWriter.MAX_U32_1;

                return resultV;
            }
            else
            {
                if (pointAt + 1 > MaxSize)
                {
                    throw SerializeException.OverSizeException;
                }

                PointAt = pointAt + 1;

                return (uint)firstV;
            }
        }


        public int ReadInt32()
        {
            int pointAt = PointAt;

            byte firstV = Bytes[pointAt];
            uint resultV = 0;
            if (firstV >= ZipBytesWriter.FLAG_U32_5_111)
            {
                if (pointAt + 5 > MaxSize)
                {
                    throw SerializeException.OverSizeException;
                }

                PointAt = pointAt + 5;

                resultV = ((uint)Bytes[pointAt + 1] << 24 | (uint)Bytes[pointAt + 2] << 16 | (uint)Bytes[pointAt + 3] << 8 | (uint)Bytes[pointAt + 4]);
                //resultV += ZipBytesWriter.MAX_32_4;

                if (firstV >= ZipBytesWriter.FLAG_32F_5_1111)
                {
                    return (int)(~resultV);
                }
                else
                {
                    return (int)resultV;
                }
            }
            else if (firstV >= ZipBytesWriter.FLAG_U32_4_110)
            {
                if (pointAt + 4 > MaxSize)
                {
                    throw SerializeException.OverSizeException;
                }

                PointAt = pointAt + 4;

                resultV = (((uint)firstV & 0xf) << 24 | (uint)Bytes[pointAt + 1] << 16 | (uint)Bytes[pointAt + 2] << 8 | (uint)Bytes[pointAt + 3]);
                resultV += ZipBytesWriter.MAX_32_3;
                if (firstV >= ZipBytesWriter.FLAG_32F_4_1101)
                {
                    return (int)(~resultV);
                }
                else
                {
                    return (int)resultV;
                }
            }
            else if (firstV >= ZipBytesWriter.FLAG_U32_3_10)
            {
                if (pointAt + 3 > MaxSize)
                {
                    throw SerializeException.OverSizeException;
                }

                PointAt = pointAt + 3;

                resultV = (((uint)firstV & 0x1f) << 16 | (uint)Bytes[pointAt + 1] << 8 | (uint)Bytes[pointAt + 2]);
                resultV += ZipBytesWriter.MAX_32_2;

                if (firstV >= ZipBytesWriter.FLAG_32F_3_101)
                {
                    return (int)(~resultV);
                }
                else
                {
                    return (int)resultV;
                }
            }
            else if (firstV >= ZipBytesWriter.FLAG_U32_2_01)
            {
                if (pointAt + 2 > MaxSize)
                {
                    throw SerializeException.OverSizeException;
                }

                PointAt = pointAt + 2;

                resultV = (((uint)firstV & 0x1f) << 8 | (uint)Bytes[pointAt + 1]);
                resultV += ZipBytesWriter.MAX_32_1;

                if (firstV >= ZipBytesWriter.FLAG_32F_2_011)
                {
                    return (int)(~resultV);
                }
                else
                {
                    return (int)resultV;
                }
            }
            else
            {
                if (pointAt + 1 > MaxSize)
                {
                    throw SerializeException.OverSizeException;
                }

                PointAt = pointAt + 1;

                resultV = (uint)firstV & 0x1f;

                if (firstV >= ZipBytesWriter.FLAG_32F_1_001)
                {
                    return (int)(~resultV);
                }
                else
                {
                    return (int)resultV;
                }
            }
        }

        public ulong ReadUInt64()
        {
            return (UInt64)ReadUInt32() << 32 | (UInt64)ReadUInt32();
        }

        public long ReadInt64()
        {
            return (Int64)ReadInt32() << 32 | (uint)ReadInt32();
        }

        public float ReadSingle()
        {
            V32 v32 = default(V32);
            v32.U32 = ReadUInt32();
            v32.U32 = v32.Reverse;

            return v32.S;
        }

        public double ReadDouble()
        {
            V64 v64 = default(V64);
            v64.U64 = ReadUInt64();
            v64.U64 = v64.Reverse;

            return v64.D;
        }

        public decimal ReadDecimal()
        {
            byte flag = ReadByte();
            ulong v1 = ReadUInt64();
            uint v2 = ReadUInt32();

            V128 v = default(V128);
            v.B02 = (byte)(flag & 0x7f);
            v.B03 = (byte)(flag & 0x80);

            v.U64_1 = v1;
            v.U32_1 = v2;

            return v.D;
        }

        public Guid ReadGuid()
        {
            V128 v = default(V128);

            v.B00 = ReadByte();
            v.B01 = ReadByte();
            v.B02 = ReadByte();
            v.B03 = ReadByte();
            v.B04 = ReadByte();
            v.B05 = ReadByte();
            v.B06 = ReadByte();
            v.B07 = ReadByte();
            v.B08 = ReadByte();
            v.B09 = ReadByte();
            v.B10 = ReadByte();
            v.B11 = ReadByte();
            v.B12 = ReadByte();
            v.B13 = ReadByte();
            v.B14 = ReadByte();
            v.B15 = ReadByte();

            return v.G;
        }

        public DateTime ReadDateTime()
        {
            V64 v = default(V64);

            v.B0 = ReadByte();
            v.B1 = ReadByte();
            v.B2 = ReadByte();
            v.B3 = ReadByte();
            v.B4 = ReadByte();
            v.B5 = ReadByte();
            v.B6 = ReadByte();
            v.B7 = ReadByte();

            DateTime result = new DateTime(v.I64 & 0x3fffffffffffffffL, (DateTimeKind)((ulong)v.I64 >> 0x3e));
            return result;
        }

        public TimeSpan ReadTimeSpan()
        {
            V64 v = default(V64);

            v.I64 = ReadInt64();

            return v.TS;
        }


        public byte TestReadByte()
        {
            int pointAt = PointAt;
            if (pointAt >= MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            byte result = Bytes[pointAt];

            return result;
        }


        public bool[] ReadBooleans()
        {
            uint len = 0;

            byte nullFlag = TestReadByte();
            if (nullFlag == BYTE_ZERO)
            {
                PointAt++;
                return null;
            }
            else
            {
                len = ReadUInt32() - 1;
            }

            int pointAt = PointAt;
            int byteLength = (int)len;
            if (pointAt + byteLength > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            bool[] result = new bool[len];

            Buffer.BlockCopy(Bytes, pointAt, result, 0, byteLength);

            PointAt = pointAt + byteLength;

            return result;
        }

        public byte[] ReadBytes()
        {
            uint len = 0;

            byte nullFlag = TestReadByte();
            if (nullFlag == BYTE_ZERO)
            {
                PointAt++;
                return null;
            }
            else
            {
                len = ReadUInt32() - 1;
            }

            int pointAt = PointAt;
            int byteLength = (int)len;
            if (pointAt + byteLength > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            byte[] result = new byte[len];

            Buffer.BlockCopy(Bytes, pointAt, result, 0, byteLength);

            PointAt = pointAt + byteLength;

            return result;
        }

        public sbyte[] ReadSBytes()
        {
            uint len = 0;

            byte nullFlag = TestReadByte();
            if (nullFlag == BYTE_ZERO)
            {
                PointAt++;
                return null;
            }
            else
            {
                len = ReadUInt32() - 1;
            }

            int pointAt = PointAt;
            int byteLength = (int)len;
            if (pointAt + byteLength > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            sbyte[] result = new sbyte[len];

            Buffer.BlockCopy(Bytes, pointAt, result, 0, byteLength);

            PointAt = pointAt + byteLength;

            return result;
        }

        public ushort[] ReadUInt16s()
        {
            uint len = 0;

            byte nullFlag = TestReadByte();
            if (nullFlag == BYTE_ZERO)
            {
                PointAt++;
                return null;
            }
            else
            {
                len = ReadUInt32() - 1;
            }

            ushort[] result = new ushort[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = ReadUInt16();
            }

            return result;
        }

        public short[] ReadInt16s()
        {
            uint len = 0;

            byte nullFlag = TestReadByte();
            if (nullFlag == BYTE_ZERO)
            {
                PointAt++;
                return null;
            }
            else
            {
                len = ReadUInt32() - 1;
            }

            short[] result = new short[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = ReadInt16();
            }

            return result;
        }

        public char[] ReadChars()
        {
            uint len = 0;

            byte nullFlag = TestReadByte();
            if (nullFlag == BYTE_ZERO)
            {
                PointAt++;
                return null;
            }
            else
            {
                len = ReadUInt32() - 1;
            }

            char[] result = new char[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = ReadChar();
            }

            return result;
        }

        public uint[] ReadUInt32s()
        {
            uint len = 0;

            byte nullFlag = TestReadByte();
            if (nullFlag == BYTE_ZERO)
            {
                PointAt++;
                return null;
            }
            else
            {
                len = ReadUInt32() - 1;
            }

            uint[] result = new uint[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = ReadUInt32();
            }

            return result;
        }

        public int[] ReadInt32s()
        {
            uint len = 0;

            byte nullFlag = TestReadByte();
            if (nullFlag == BYTE_ZERO)
            {
                PointAt++;
                return null;
            }
            else
            {
                len = ReadUInt32() - 1;
            }

            int[] result = new int[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = ReadInt32();
            }

            return result;
        }

        public ulong[] ReadUInt64s()
        {
            uint len = 0;

            byte nullFlag = TestReadByte();
            if (nullFlag == BYTE_ZERO)
            {
                PointAt++;
                return null;
            }
            else
            {
                len = ReadUInt32() - 1;
            }

            ulong[] result = new ulong[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = ReadUInt64();
            }

            return result;
        }

        public long[] ReadInt64s()
        {
            uint len = 0;

            byte nullFlag = TestReadByte();
            if (nullFlag == BYTE_ZERO)
            {
                PointAt++;
                return null;
            }
            else
            {
                len = ReadUInt32() - 1;
            }

            long[] result = new long[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = ReadInt64();
            }

            return result;
        }

        public float[] ReadSingles()
        {
            uint len = 0;

            byte nullFlag = TestReadByte();
            if (nullFlag == BYTE_ZERO)
            {
                PointAt++;
                return null;
            }
            else
            {
                len = ReadUInt32() - 1;
            }

            float[] result = new float[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = ReadSingle();
            }

            return result;
        }

        public double[] ReadDoubles()
        {
            uint len = 0;

            byte nullFlag = TestReadByte();
            if (nullFlag == BYTE_ZERO)
            {
                PointAt++;
                return null;
            }
            else
            {
                len = ReadUInt32() - 1;
            }

            double[] result = new double[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = ReadDouble();
            }

            return result;
        }

        public decimal[] ReadDecimals()
        {
            uint len = 0;

            byte nullFlag = TestReadByte();
            if (nullFlag == BYTE_ZERO)
            {
                PointAt++;
                return null;
            }
            else
            {
                len = ReadUInt32() - 1;
            }

            decimal[] result = new decimal[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = ReadDecimal();
            }

            return result;
        }

        public Guid[] ReadGuids()
        {
            uint len = 0;

            byte nullFlag = TestReadByte();
            if (nullFlag == BYTE_ZERO)
            {
                PointAt++;
                return null;
            }
            else
            {
                len = ReadUInt32() - 1;
            }

            Guid[] result = new Guid[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = ReadGuid();
            }

            return result;
        }

        public DateTime[] ReadDateTimes()
        {
            uint len = 0;

            byte nullFlag = TestReadByte();
            if (nullFlag == BYTE_ZERO)
            {
                PointAt++;
                return null;
            }
            else
            {
                len = ReadUInt32() - 1;
            }

            DateTime[] result = new DateTime[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = ReadDateTime();
            }

            return result;
        }

        public TimeSpan[] ReadTimeSpans()
        {
            uint len = 0;

            byte nullFlag = TestReadByte();
            if (nullFlag == BYTE_ZERO)
            {
                PointAt++;
                return null;
            }
            else
            {
                len = ReadUInt32() - 1;
            }

            TimeSpan[] result = new TimeSpan[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = ReadTimeSpan();
            }

            return result;
        }

        public string ReadString()
        {
            uint len = 0;

            byte nullFlag = TestReadByte();
            if (nullFlag == BYTE_ZERO)
            {
                PointAt++;
                return null;
            }
            else
            {
                len = ReadUInt32() - 1;
            }

            int pointAt = PointAt;
            int byteLength = (int)len;
            if (pointAt + byteLength > MaxSize)
            {
                throw SerializeException.OverSizeException;
            }

            byte[] result = new byte[len];

            Buffer.BlockCopy(Bytes, pointAt, result, 0, byteLength);

            PointAt = pointAt + byteLength;

            return System.Text.Encoding.UTF8.GetString(result, 0, byteLength);
        }
    }
}
