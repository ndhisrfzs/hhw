using System;

namespace GN
{
    /// <summary>
    /// 将基础值类型（和对应数组）写入byte数组
    /// </summary>
    public class ZipBytesWriter
    {
        internal const int DEFAULT_BYTESLENGTH = 128;

        internal const byte BYTE_ZERO = 0;

        internal const byte BYTE_ONE = 0x1;

        
        //int32
        //UInt32头两个bit位，Int32加一个bit表示正负
        //00:共一个byte表示数据，01:共2个byte表示数据，10:共3个byte表示数据，110:共4个byte表示数据, 111:共5个byte表示数据
        internal const UInt32 MAX_U32_1 = 0x3f;
        internal const UInt32 MAX_32_1 = 0x1f;
        internal const byte FLAG_32F_1_001 = 0x20;

        internal const UInt32 MAX_U32_2 = 0x3fff + MAX_U32_1;
        internal const UInt32 MAX_32_2 = 0x1fff + MAX_32_1;
        internal const byte FLAG_U32_2_01 = 0x40;
        internal const byte FLAG_32F_2_011 = 0x60;

        internal const UInt32 MAX_U32_3 = 0x3fffff + MAX_U32_2;
        internal const UInt32 MAX_32_3 = 0x1fffff + MAX_32_2;
        internal const byte FLAG_U32_3_10 = 0x80;
        internal const byte FLAG_32F_3_101 = 0xa0;

        internal const UInt32 MAX_U32_4 = 0x1fffffff + MAX_U32_3;
        internal const UInt32 MAX_32_4 = 0xfffffff + MAX_32_3;
        internal const byte FLAG_U32_4_110 = 0xc0;
        internal const byte FLAG_32F_4_1101 = 0xd0;

        internal const byte FLAG_U32_5_111 = 0xe0;
        internal const byte FLAG_32F_5_1111 = 0xf0;

        

        /// <summary>
        /// 获取有效的byte数组长度
        /// </summary>
        public int EffectiveBytesLength
        {
            get { return PointAt; }
        }

        /// <summary>
        /// 当前将要写入的Bytes的index
        /// </summary>
        private int PointAt;


        /// <summary>
        /// 当前Bytes的Capacity
        /// </summary>
        private int Capacity;


        /// <summary>
        /// 允许写入的最大长度
        /// </summary>
        private int F_MaxSize;

        /// <summary>
        /// 允许写入的最大长度
        /// </summary>
        public int MaxSize
        {
            get { return F_MaxSize; }
        }


        /// <summary>
        /// 结果Bytes
        /// </summary>
        private byte[] F_Bytes;

        /// <summary>
        /// 结果Bytes
        /// </summary>
        public byte[] Bytes
        {
            get { return F_Bytes; }
        }


        /// <summary>
        /// 根据一个推测的大小初始化BytesWriter
        /// </summary>
        /// <param name="capacity">推测的大小</param>
        /// <param name="maxSize">当前操作的byte[]所允许的最大长度</param>
        public ZipBytesWriter(int capacity, int maxSize)
        {
            if (capacity > maxSize)
            {
                throw new ArgumentOutOfRangeException("capacity must <= size");
            }
            Capacity = capacity;
            F_MaxSize = maxSize;
            F_Bytes = new byte[capacity];
        }

        /// <summary>
        /// 根据默认初始大小初始化BytesWriter
        /// </summary>
        /// <param name="maxSize">当前操作的byte[]所允许的最大长度</param>
        public ZipBytesWriter(int maxSize)
        {
            Capacity = Math.Min(DEFAULT_BYTESLENGTH, maxSize);
            F_MaxSize = maxSize;
            F_Bytes = new byte[Capacity];
        }


        /// <summary>
        /// 提供一个原始byte数组，BytesWriter将在这个数组startWriteIndex位置开始写入操作
        /// </summary>
        /// <param name="bytesBuf">原始的byte数组，数据会被保留</param>
        /// <param name="startWriteIndex">从哪个位置开始，注意，如果这个值小于bytesBuf.Length，将不会进行一次额外的内存拷贝工作</param>
        /// <param name="maxSize">当前操作的byte[]所允许的最大长度</param>
        public ZipBytesWriter(byte[] bytesBuf, int startWriteIndex, int maxSize)
        {
            int length = bytesBuf.Length;
            if (startWriteIndex >= maxSize)
            {
                throw new ArgumentOutOfRangeException("startWriteIndex must <= size");
            }
            if (startWriteIndex >= length)
            {
                Capacity = Math.Min(startWriteIndex * 3 / 2, maxSize);
                byte[] newBytes = new byte[Capacity];
                Buffer.BlockCopy(bytesBuf, 0, newBytes, 0, length);
                F_Bytes = newBytes;
            }
            else
            {
                Capacity = length;
                F_Bytes = bytesBuf;
            }
            F_MaxSize = maxSize;

            PointAt = startWriteIndex;
        }


        /// <summary>
        /// 动态增加可写入大小
        /// </summary>
        /// <param name="addValue">增加的值</param>
        /// <returns>返回新值</returns>
        public int AddMaxSize(int addValue)
        {
            int newValue = F_MaxSize + addValue;
            if (newValue < F_MaxSize)
            {
                throw new ArgumentException("addValue must >= 0");
            }
            F_MaxSize = newValue;
            return newValue;
        }


        /// <summary>
        /// 检查并递增Bytes尺寸
        /// </summary>
        /// <param name="willAddLength">将要写入的长度</param>
        private void CheckAndReSize(int willAddLength)
        {
            int willLength = PointAt + willAddLength;
            if (willLength > F_MaxSize)
            {
                throw SerializeException.OverSizeException;
            }
            if (willLength > Capacity)
            {
                Capacity = Capacity * 3 / 2 + willAddLength;
                byte[] newBytes = new byte[Capacity];
                Buffer.BlockCopy(F_Bytes, 0, newBytes, 0, PointAt);
                F_Bytes = newBytes;

                //旧数组交由垃圾收集器处理
            }
        }


        public static void WriteAtUshort(byte[] bytes, int index, UInt16 value)
        {
            bytes[index] = (byte)(value >> 8);
            bytes[index + 1] = (byte)(value);
        }

        public void Write(bool value)
        {
            if (value)
            {
                Write(BYTE_ONE);
            }
            else
            {
                Write(BYTE_ZERO);
            }
        }




        public void Write(byte value)
        {
            CheckAndReSize(1);

            F_Bytes[PointAt] = value;

            PointAt++;
        }


        public void Write(sbyte value)
        {
            CheckAndReSize(1);

            F_Bytes[PointAt] = (byte)value;

            PointAt++;
        }


        public void Write(UInt16 value)
        {
            CheckAndReSize(2);

            F_Bytes[PointAt] = (byte)(value >> 8);
            F_Bytes[PointAt + 1] = (byte)value;

            PointAt += 2;
        }


        public void Write(Int16 value)
        {
            CheckAndReSize(2);

            F_Bytes[PointAt] = (byte)(value >> 8);
            F_Bytes[PointAt + 1] = (byte)value;

            PointAt += 2;
        }




        public void Write(Char value)
        {
            Write((UInt16)value);
        }

        public void Write(UInt32 value)
        {
            if (value <= MAX_U32_1)
            {
                CheckAndReSize(1);

                F_Bytes[PointAt] = (byte)value;

                PointAt++;
            }
            else if (value <= MAX_U32_2)
            {
                CheckAndReSize(2);

                value -= MAX_U32_1;

                F_Bytes[PointAt] = (byte)((value >> 8) | FLAG_U32_2_01);
                F_Bytes[PointAt + 1] = (byte)value;

                PointAt += 2;
            }
            else if (value <= MAX_U32_3)
            {
                CheckAndReSize(3);

                value -= MAX_U32_2;

                F_Bytes[PointAt] = (byte)((value >> 16) | FLAG_U32_3_10);
                F_Bytes[PointAt + 1] = (byte)(value >> 8);
                F_Bytes[PointAt + 2] = (byte)value;

                PointAt += 3;
            }
            else if (value <= MAX_U32_4)
            {
                CheckAndReSize(4);

                value -= MAX_U32_3;

                F_Bytes[PointAt] = (byte)((value >> 24) | FLAG_U32_4_110);
                F_Bytes[PointAt + 1] = (byte)(value >> 16);
                F_Bytes[PointAt + 2] = (byte)(value >> 8);
                F_Bytes[PointAt + 3] = (byte)value;

                PointAt += 4;
            }
            else
            {
                CheckAndReSize(5);

                F_Bytes[PointAt] = FLAG_U32_5_111;
                F_Bytes[PointAt + 1] = (byte)(value >> 24);
                F_Bytes[PointAt + 2] = (byte)(value >> 16);
                F_Bytes[PointAt + 3] = (byte)(value >> 8);
                F_Bytes[PointAt + 4] = (byte)value;

                PointAt += 5;
            }
            
        }


        public void WriteRawUInt(UInt32 value)
        {
            CheckAndReSize(4);

            F_Bytes[PointAt] = (byte)(value >> 24);
            F_Bytes[PointAt + 1] = (byte)(value >> 16);
            F_Bytes[PointAt + 2] = (byte)(value >> 8);
            F_Bytes[PointAt + 3] = (byte)value;

            PointAt += 4;
        }


        public void Write(Int32 value)
        {
            bool isF = false;
            uint nV = 0;

            if (value >= 0)
            {
                nV = (uint)value;
            }
            else
            {
                isF = true;
                nV = (uint)(~value);
            }
            if (nV <= MAX_32_1)
            {
                CheckAndReSize(1);
                if (isF)
                {
                    F_Bytes[PointAt] = (byte)(nV | FLAG_32F_1_001);
                }
                else
                {
                    F_Bytes[PointAt] = (byte)nV;
                }
                PointAt++;
            }
            else if (nV <= MAX_32_2)
            {
                CheckAndReSize(2);

                nV -= MAX_32_1;

                if (isF)
                {
                    F_Bytes[PointAt] = (byte)((nV >> 8) | FLAG_32F_2_011);
                }
                else
                {
                    F_Bytes[PointAt] = (byte)((nV >> 8) | FLAG_U32_2_01);
                }
                F_Bytes[PointAt + 1] = (byte)nV;
                
                PointAt += 2;
            }
            else if (nV <= MAX_32_3)
            {
                CheckAndReSize(3);

                nV -= MAX_32_2;

                if (isF)
                {
                    F_Bytes[PointAt] = (byte)((nV >> 16) | FLAG_32F_3_101);
                }
                else
                {
                    F_Bytes[PointAt] = (byte)((nV >> 16) | FLAG_U32_3_10);
                }
                F_Bytes[PointAt + 1] = (byte)(nV >> 8);
                F_Bytes[PointAt + 2] = (byte)nV;
                
                PointAt += 3;
            }
            else if (nV < MAX_32_4)
            {
                CheckAndReSize(4);

                nV -= MAX_32_3;
                
                if (isF)
                {
                    F_Bytes[PointAt] = (byte)((nV >> 24) | FLAG_32F_4_1101);
                }
                else
                {
                    F_Bytes[PointAt] = (byte)((nV >> 24) | FLAG_U32_4_110);
                }
                F_Bytes[PointAt + 1] = (byte)(nV >> 16);
                F_Bytes[PointAt + 2] = (byte)(nV >> 8);
                F_Bytes[PointAt + 3] = (byte)nV;

                PointAt += 4;
            }
            else
            {
                CheckAndReSize(5);
                
                if (isF)
                {
                    F_Bytes[PointAt] = FLAG_32F_5_1111;
                }
                else
                {
                    F_Bytes[PointAt] = FLAG_U32_5_111;
                }
                F_Bytes[PointAt + 1] = (byte)(nV >> 24);
                F_Bytes[PointAt + 2] = (byte)(nV >> 16);
                F_Bytes[PointAt + 3] = (byte)(nV >> 8);
                F_Bytes[PointAt + 4] = (byte)nV;

                PointAt += 5;
            }
        }





        public void Write(UInt64 value)
        {
            UInt32 v0 = (UInt32)value;
            UInt32 v1 = (UInt32)(value >> 32);

            Write(v1);
            Write(v0);
        }




        public void Write(Int64 value)
        {
            Int32 v0 = (Int32)value;
            Int32 v1 = (Int32)(value >> 32);

            Write(v1);
            Write(v0);
        }




        public void Write(Single value)
        {
            V32 v = default(V32);
            v.S = value;
            Write(v.Reverse);  //Reverse后可能得到更小的值
        }



        public void Write(Double value)
        {
            V64 v = default(V64);
            v.D = value;
            Write(v.Reverse);  //Reverse后可能得到更小的值
        }





        public void Write(Decimal value)
        {
            V128 v = default(V128);
            v.D = value;

            //将表示正负的位置合并到表示小数点位置的地方
            //v.B03 == 0表示正数，128表示负数
            //v.B02表示小数点位置（从右算起）
            Write((byte)(v.B02 | v.B03));

            //写入第一个值
            Write(v.U64_1);

            //写入附加值（这个值为0的可能性很大）
            Write(v.U32_1);
        }




        public void Write(Guid value)
        {
            V128 v = default(V128);
            v.G = value;

            Write(v.B00);
            Write(v.B01);
            Write(v.B02);
            Write(v.B03);
            Write(v.B04);
            Write(v.B05);
            Write(v.B06);
            Write(v.B07);
            Write(v.B08);
            Write(v.B09);
            Write(v.B10);
            Write(v.B11);
            Write(v.B12);
            Write(v.B13);
            Write(v.B14);
            Write(v.B15);
        }




        public void Write(DateTime value)
        {
            V64 v = default(V64);

            long dt = value.Ticks | (long)value.Kind << 0x3e;
            v.I64 = dt;

            Write(v.B0);
            Write(v.B1);
            Write(v.B2);
            Write(v.B3);
            Write(v.B4);
            Write(v.B5);
            Write(v.B6);
            Write(v.B7);
        }




        public void Write(TimeSpan value)
        {
            V64 v = default(V64);
            v.TS = value;

            Write(v.I64);
        }

        /// <summary>
        /// 写入一个引用类型是否是Null的标记头
        /// </summary>
        /// <param name="isNull"></param>
        public void WriteNullHead(bool isNull)
        {
            if (isNull)
            {
                Write(BYTE_ZERO);
            }
            else
            {
                Write(BYTE_ONE);
            }
        }

        /// <summary>
        /// 写入一个数组（或者IList）类型的头数据，包括是否为null，不是null的实际长度
        /// </summary>
        /// <param name="count">如果为null，赋值为-1。其他请赋值为实际长度</param>
        public void WriteArrayHead(int count)
        {
            if (count < 0)
            {
                Write(BYTE_ZERO);
            }
            else
            {
                Write(BYTE_ONE);
                Write((uint)count);
            }
        }


        public void Write(bool[] value)
        {
            if (value == null)
            {
                Write(BYTE_ZERO);
                return;
            }

            uint uLength = (uint)value.Length;
            Write(uLength + 1u);

            CheckAndReSize((int)uLength);

            Buffer.BlockCopy(value, 0, F_Bytes, PointAt, (int)uLength);

            PointAt += (int)uLength;
        }





        public void Write(byte[] value, int valueLength)
        {
            if (value == null || valueLength < 0)
            {
                Write(BYTE_ZERO);
                return;
            }


            uint uLength = (uint)valueLength;
            Write(uLength + 1u);

            CheckAndReSize(valueLength);

            Buffer.BlockCopy(value, 0, F_Bytes, PointAt, valueLength);

            PointAt += valueLength;
        }





        public void Write(sbyte[] value)
        {
            if (value == null)
            {
                Write(BYTE_ZERO);
                return;
            }

            uint uLength = (uint)value.Length;
            Write(uLength + 1u);

            CheckAndReSize((int)uLength);

            Buffer.BlockCopy(value, 0, F_Bytes, PointAt, (int)uLength);

            PointAt += (int)uLength;
        }






        public void Write(UInt16[] value)
        {
            if (value == null)
            {
                Write(BYTE_ZERO);
                return;
            }

            uint uLength = (uint)value.Length;
            Write(uLength + 1u);

            for (int i = 0; i < uLength; i++)
            {
                Write(value[i]);
            }
        }





        public void Write(Int16[] value)
        {
            if (value == null)
            {
                Write(BYTE_ZERO);
                return;
            }

            uint uLength = (uint)value.Length;
            Write(uLength + 1u);

            for (int i = 0; i < uLength; i++)
            {
                Write(value[i]);
            }
        }






        public void Write(Char[] value)
        {
            if (value == null)
            {
                Write(BYTE_ZERO);
                return;
            }

            uint uLength = (uint)value.Length;
            Write(uLength + 1u);

            for (int i = 0; i < uLength; i++)
            {
                Write(value[i]);
            }
        }





        public void Write(UInt32[] value)
        {
            if (value == null)
            {
                Write(BYTE_ZERO);
                return;
            }

            uint uLength = (uint)value.Length;
            Write(uLength + 1u);

            for (int i = 0; i < uLength; i++)
            {
                Write(value[i]);
            }
        }





        public void Write(Int32[] value)
        {
            if (value == null)
            {
                Write(BYTE_ZERO);
                return;
            }

            uint uLength = (uint)value.Length;
            Write(uLength + 1u);

            for (int i = 0; i < uLength; i++)
            {
                Write(value[i]);
            }
        }




        public void Write(UInt64[] value)
        {
            if (value == null)
            {
                Write(BYTE_ZERO);
                return;
            }

            uint uLength = (uint)value.Length;
            Write(uLength + 1u);
            for (int i = 0; i < uLength; i++)
            {
                Write(value[i]);
            }
        }




        public void Write(Int64[] value)
        {
            if (value == null)
            {
                Write(BYTE_ZERO);
                return;
            }

            uint uLength = (uint)value.Length;
            Write(uLength + 1u);

            for (int i = 0; i < uLength; i++)
            {
                Write(value[i]);
            }
        }





        public void Write(Single[] value)
        {
            if (value == null)
            {
                Write(BYTE_ZERO);
                return;
            }

            uint uLength = (uint)value.Length;
            Write(uLength + 1u);
            
            for (int i = 0; i < uLength; i++)
            {
                Write(value[i]);
            }            
        }





        public void Write(Double[] value)
        {
            if (value == null)
            {
                Write(BYTE_ZERO);
                return;
            }

            uint uLength = (uint)value.Length;
            Write(uLength + 1u);
            
            for (int i = 0; i < uLength; i++)
            {
                Write(value[i]);
            }            
        }






        public void Write(Decimal[] value)
        {
            if (value == null)
            {
                Write(BYTE_ZERO);
                return;
            }

            uint uLength = (uint)value.Length;
            Write(uLength + 1u);
            for (int i = 0; i < uLength; i++)
            {
                Write(value[i]);
            }
        }



        public void Write(Guid[] value)
        {
            if (value == null)
            {
                Write(BYTE_ZERO);
                return;
            }

            uint uLength = (uint)value.Length;
            Write(uLength + 1u);
            for (int i = 0; i < uLength; i++)
            {
                Write(value[i]);
            }
        }





        public void Write(DateTime[] value)
        {
            if (value == null)
            {
                Write(BYTE_ZERO);
                return;
            }

            uint uLength = (uint)value.Length;
            Write(uLength + 1u);
            for (int i = 0; i < uLength; i++)
            {
                Write(value[i]);
            }
        }





        public void Write(TimeSpan[] value)
        {
            if (value == null)
            {
                Write(BYTE_ZERO);
                return;
            }

            uint uLength = (uint)value.Length;
            Write(uLength + 1u);
            for (int i = 0; i < uLength; i++)
            {
                Write(value[i]);
            }
        }




        public void Write(string value)
        {
            if (value == null)
            {
                Write(BYTE_ZERO);
                return;
            }

            byte[] utf8Code = System.Text.Encoding.UTF8.GetBytes(value);
            int length = utf8Code.Length;

            Write((uint)length + 1u);

            CheckAndReSize(length);

            Buffer.BlockCopy(utf8Code, 0, F_Bytes, PointAt, length);

            PointAt += length;
        }
    }
}
