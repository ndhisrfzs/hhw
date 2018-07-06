using System;

namespace GN
{
    /// <summary>
    /// 将基础值类型（和对应数组）写入byte数组
    /// </summary>
    public class BytesWriter
    {
        private const int DEFAULT_BYTESLENGTH = 128;

        private const byte Byte_Zero = 0;

        private const byte Byte_One = 0xff;


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
        public byte[] F_Bytes;

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
        public BytesWriter(int capacity, int maxSize)
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
        public BytesWriter(int maxSize)
        {
            Capacity = Math.Min(DEFAULT_BYTESLENGTH, maxSize);
            F_MaxSize = maxSize;
            F_Bytes = new byte[Capacity];
        }


        /// <summary>
        /// 提供一个原始byte数组，BytesWriter将在这个数组最后开始写入操作
        /// </summary>
        /// <param name="bytesBuf">原始的byte数组，数据会被保留</param>
        /// <param name="startWriteIndex">从哪个位置开始，注意，如果这个值小于bytesBuf.Length，将不会进行一次额外的内存拷贝工作</param>
        /// <param name="maxSize">当前操作的byte[]所允许的最大长度</param>
        public BytesWriter(byte[] bytesBuf, int startWriteIndex, int maxSize)
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



        public static void WriteAt(byte[] bytes, int index, bool value)
        {
            bytes[index] = (value) ? Byte_One : Byte_Zero;
        }
        public void Write(bool value)
        {
            CheckAndReSize(1);

            if (value)
            {
                F_Bytes[PointAt] = 1;
            }
            else
            {
                F_Bytes[PointAt] = 0;
            }

            PointAt++;
        }




        public static void WriteAt(byte[] bytes, int index, byte value)
        {
            bytes[index] = value;
        }
        public void Write(byte value)
        {
            CheckAndReSize(1);

            F_Bytes[PointAt] = value;

            PointAt++;
        }




        public static void WriteAt(byte[] bytes, int index, sbyte value)
        {
            bytes[index] = (byte)value;
        }
        public void Write(sbyte value)
        {
            CheckAndReSize(1);

            F_Bytes[PointAt] = (byte)value;

            PointAt++;
        }




        public static void WriteAt(byte[] bytes, int index, UInt16 value)
        {
            bytes[index] = (byte)value;
            bytes[index + 1] = (byte)(value >> 8);
        }
        public void Write(UInt16 value)
        {
            CheckAndReSize(2);

            F_Bytes[PointAt] = (byte)value;
            F_Bytes[PointAt + 1] = (byte)(value >> 8);

            PointAt += 2;
        }




        public static void WriteAt(byte[] bytes, int index, Int16 value)
        {
            bytes[index] = (byte)value;
            bytes[index + 1] = (byte)(value >> 8);
        }
        public void Write(Int16 value)
        {
            CheckAndReSize(2);

            F_Bytes[PointAt] = (byte)value;
            F_Bytes[PointAt + 1] = (byte)(value >> 8);

            PointAt += 2;
        }




        public static void WriteAt(byte[] bytes, int index, Char value)
        {
            bytes[index] = (byte)value;
            bytes[index + 1] = (byte)(value >> 8);
        }
        public void Write(Char value)
        {
            CheckAndReSize(2);

            F_Bytes[PointAt] = (byte)value;
            F_Bytes[PointAt + 1] = (byte)(value >> 8);

            PointAt += 2;
        }





        public static void WriteAt(byte[] bytes, int index, UInt32 value)
        {
            bytes[index] = (byte)value;
            bytes[index + 1] = (byte)(value >> 8);
            bytes[index + 2] = (byte)(value >> 16);
            bytes[index + 3] = (byte)(value >> 24);
        }
        public void Write(UInt32 value)
        {
            CheckAndReSize(4);

            Bytes[PointAt] = (byte)value;
            Bytes[PointAt + 1] = (byte)(value >> 8);
            Bytes[PointAt + 2] = (byte)(value >> 16);
            Bytes[PointAt + 3] = (byte)(value >> 24);

            PointAt += 4;
        }




        public static void WriteAt(byte[] bytes, int index, Int32 value)
        {
            bytes[index] = (byte)value;
            bytes[index + 1] = (byte)(value >> 8);
            bytes[index + 2] = (byte)(value >> 16);
            bytes[index + 3] = (byte)(value >> 24);
        }
        public void Write(Int32 value)
        {
            CheckAndReSize(4);

            Bytes[PointAt] = (byte)value;
            Bytes[PointAt + 1] = (byte)(value >> 8);
            Bytes[PointAt + 2] = (byte)(value >> 16);
            Bytes[PointAt + 3] = (byte)(value >> 24);

            PointAt += 4;
        }






        public static void WriteAt(byte[] bytes, int index, UInt64 value)
        {
            bytes[index] = (byte)value;
            bytes[index + 1] = (byte)(value >> 8);
            bytes[index + 2] = (byte)(value >> 16);
            bytes[index + 3] = (byte)(value >> 24);
            bytes[index + 4] = (byte)(value >> 32);
            bytes[index + 5] = (byte)(value >> 40);
            bytes[index + 6] = (byte)(value >> 48);
            bytes[index + 7] = (byte)(value >> 56);
        }
        public void Write(UInt64 value)
        {
            CheckAndReSize(8);

            Bytes[PointAt] = (byte)value;
            Bytes[PointAt + 1] = (byte)(value >> 8);
            Bytes[PointAt + 2] = (byte)(value >> 16);
            Bytes[PointAt + 3] = (byte)(value >> 24);
            Bytes[PointAt + 4] = (byte)(value >> 32);
            Bytes[PointAt + 5] = (byte)(value >> 40);
            Bytes[PointAt + 6] = (byte)(value >> 48);
            Bytes[PointAt + 7] = (byte)(value >> 56);

            PointAt += 8;
        }




        public static void WriteAt(byte[] bytes, int index, Int64 value)
        {
            bytes[index] = (byte)value;
            bytes[index + 1] = (byte)(value >> 8);
            bytes[index + 2] = (byte)(value >> 16);
            bytes[index + 3] = (byte)(value >> 24);
            bytes[index + 4] = (byte)(value >> 32);
            bytes[index + 5] = (byte)(value >> 40);
            bytes[index + 6] = (byte)(value >> 48);
            bytes[index + 7] = (byte)(value >> 56);
        }
        public void Write(Int64 value)
        {
            CheckAndReSize(8);

            Bytes[PointAt] = (byte)value;
            Bytes[PointAt + 1] = (byte)(value >> 8);
            Bytes[PointAt + 2] = (byte)(value >> 16);
            Bytes[PointAt + 3] = (byte)(value >> 24);
            Bytes[PointAt + 4] = (byte)(value >> 32);
            Bytes[PointAt + 5] = (byte)(value >> 40);
            Bytes[PointAt + 6] = (byte)(value >> 48);
            Bytes[PointAt + 7] = (byte)(value >> 56);

            PointAt += 8;
        }




        public static void WriteAt(byte[] bytes, int index, Single value)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, bytes, index, 4);
        }
        public void Write(Single value)
        {
            CheckAndReSize(4);

            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, Bytes, PointAt, 4);

            PointAt += 4;
        }




        public static void WriteAt(byte[] bytes, int index, Double value)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, bytes, index, 8);
        }

        public void Write(Double value)
        {
            CheckAndReSize(8);

            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, Bytes, PointAt, 8);

            PointAt += 8;
        }





        public static void WriteAt(byte[] bytes, int index, Decimal value)
        {
            int[] bits = decimal.GetBits(value);
            int flags = bits[3];
            int hi = bits[2];
            int lo = bits[0];
            int mid = bits[1];
            bytes[index] = (byte)flags;
            bytes[index + 1] = (byte)(flags >> 8);
            bytes[index + 2] = (byte)(flags >> 16);
            bytes[index + 3] = (byte)(flags >> 24);
            bytes[index + 4] = (byte)hi;
            bytes[index + 5] = (byte)(hi >> 8);
            bytes[index + 6] = (byte)(hi >> 16);
            bytes[index + 7] = (byte)(hi >> 24);
            bytes[index + 8] = (byte)lo;
            bytes[index + 9] = (byte)(lo >> 8);
            bytes[index + 10] = (byte)(lo >> 16);
            bytes[index + 11] = (byte)(lo >> 24);
            bytes[index + 12] = (byte)mid;
            bytes[index + 13] = (byte)(mid >> 8);
            bytes[index + 14] = (byte)(mid >> 16);
            bytes[index + 15] = (byte)(mid >> 24);
        }

        public void Write(Decimal value)
        {
            CheckAndReSize(16);

            int[] bits = decimal.GetBits(value);
            int flags = bits[3];
            int hi = bits[2];
            int lo = bits[0];
            int mid = bits[1];
            Bytes[PointAt] = (byte)flags;
            Bytes[PointAt + 1] = (byte)(flags >> 8);
            Bytes[PointAt + 2] = (byte)(flags >> 16);
            Bytes[PointAt + 3] = (byte)(flags >> 24);
            Bytes[PointAt + 4] = (byte)hi;
            Bytes[PointAt + 5] = (byte)(hi >> 8);
            Bytes[PointAt + 6] = (byte)(hi >> 16);
            Bytes[PointAt + 7] = (byte)(hi >> 24);
            Bytes[PointAt + 8] = (byte)lo;
            Bytes[PointAt + 9] = (byte)(lo >> 8);
            Bytes[PointAt + 10] = (byte)(lo >> 16);
            Bytes[PointAt + 11] = (byte)(lo >> 24);
            Bytes[PointAt + 12] = (byte)mid;
            Bytes[PointAt + 13] = (byte)(mid >> 8);
            Bytes[PointAt + 14] = (byte)(mid >> 16);
            Bytes[PointAt + 15] = (byte)(mid >> 24);

            PointAt += 16;
        }




        public static void WriteAt(byte[] bytes, int index, Guid value)
        {
            Buffer.BlockCopy(value.ToByteArray(), 0, bytes, index, 16);
        }


        public void Write(Guid value)
        {
            CheckAndReSize(16);

            Buffer.BlockCopy(value.ToByteArray(), 0, Bytes, PointAt, 16);

            PointAt += 16;
        }




        public static void WriteAt(byte[] bytes, int index, DateTime value)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(value.Ticks | (long)value.Kind << 0x3e), 0, bytes, index, 8);
        }
        public void Write(DateTime value)
        {
            CheckAndReSize(8);

            Buffer.BlockCopy(BitConverter.GetBytes(value.Ticks | (long)value.Kind << 0x3e), 0, Bytes, PointAt, 8);

            PointAt += 8;
        }


        public void WriteNullHead(bool isNull)
        {
            if (isNull)
            {
                Write(Byte_Zero);
            }
            else
            {
                Write(Byte_One);
            }
        }



        /// <summary>
        /// 写入一个数组（或者IList）类型的头数据，包括是否为null，不是null的实际长度
        /// null将写入一个byte 0x00。非null将写入0xff，并在之后写入int32(4byte)的长度数值
        /// </summary>
        /// <param name="count">如果为null，赋值为-1。其他请赋值为实际长度</param>
        public void WriteArrayHead(int count)
        {
            if (count < 0)
            {
                Write(Byte_Zero);
            }
            else
            {
                Write(Byte_One);
                Write(count);
            }
        }



        public static void WriteAt(byte[] bytes, int index, TimeSpan value)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(value.Ticks), 0, bytes, index, 8);
        }
        public void Write(TimeSpan value)
        {
            CheckAndReSize(8);

            Buffer.BlockCopy(BitConverter.GetBytes(value.Ticks), 0, Bytes, PointAt, 8);

            PointAt += 8;
        }




        public static void WriteAt(byte[] bytes, int index, bool[] value)
        {
            if (value == null)
            {
                bytes[index] = Byte_Zero;
                return;
            }

            int byteLength = value.Length;

            bytes[index] = Byte_One;
            WriteAt(bytes, index + 1, byteLength);

            Buffer.BlockCopy(value, 0, bytes, index + 5, byteLength);

        }
        public void Write(bool[] value)
        {
            if (value == null)
            {
                Write(Byte_Zero);
                return;
            }

            int byteLength = value.Length;
            CheckAndReSize(byteLength + 5);

            Write(Byte_One);
            Write(byteLength);

            Buffer.BlockCopy(value, 0, Bytes, PointAt, byteLength);

            PointAt += byteLength;
        }






        public static void WriteAt(byte[] bytes, int index, byte[] value, int valueLength)
        {
            if (value == null)
            {
                bytes[index] = Byte_Zero;
                return;
            }

            bytes[index] = Byte_One;
            WriteAt(bytes, index + 1, valueLength);

            Buffer.BlockCopy(value, 0, bytes, index + 5, valueLength);
        }
        public void Write(byte[] value, int valueLength)
        {
            if (value == null)
            {
                Write(Byte_Zero);
                return;
            }

            CheckAndReSize(valueLength + 5);

            Write(Byte_One);
            Write(valueLength);

            Buffer.BlockCopy(value, 0, Bytes, PointAt, valueLength);

            PointAt += valueLength;
        }


        public void WriteRaw(byte[] value, int valueLength)
        {
            if (value == null)
            {
                return;
            }

            CheckAndReSize(valueLength);

            Buffer.BlockCopy(value, 0, Bytes, PointAt, valueLength);

            PointAt += valueLength;
        }






        public static void WriteAt(byte[] bytes, int index, sbyte[] value)
        {
            if (value == null)
            {
                bytes[index] = Byte_Zero;
                return;
            }

            int byteLength = value.Length;

            bytes[index] = Byte_One;
            WriteAt(bytes, index + 1, byteLength);

            Buffer.BlockCopy(value, 0, bytes, index + 5, byteLength);
        }
        public void Write(sbyte[] value)
        {
            if (value == null)
            {
                Write(Byte_Zero);
                return;
            }

            int byteLength = value.Length;
            CheckAndReSize(byteLength + 5);

            Write(Byte_One);
            Write(byteLength);

            Buffer.BlockCopy(value, 0, Bytes, PointAt, byteLength);

            PointAt += byteLength;
        }





        public static void WriteAt(byte[] bytes, int index, UInt16[] value)
        {
            if (value == null)
            {
                bytes[index] = Byte_Zero;
                return;
            }

            int byteLength = value.Length << 1;

            bytes[index] = Byte_One;
            WriteAt(bytes, index + 1, byteLength);

            Buffer.BlockCopy(value, 0, bytes, index + 5, byteLength);

        }
        public void Write(UInt16[] value)
        {
            if (value == null)
            {
                Write(Byte_Zero);
                return;
            }

            int byteLength = value.Length << 1;
            CheckAndReSize(byteLength + 5);

            Write(Byte_One);
            Write(byteLength);

            Buffer.BlockCopy(value, 0, Bytes, PointAt, byteLength);

            PointAt += byteLength;
        }





        public static void WriteAt(byte[] bytes, int index, Int16[] value)
        {
            if (value == null)
            {
                bytes[index] = Byte_Zero;
                return;
            }

            int byteLength = value.Length << 1;

            bytes[index] = Byte_One;
            WriteAt(bytes, index + 1, byteLength);

            Buffer.BlockCopy(value, 0, bytes, index + 5, byteLength);

        }
        public void Write(Int16[] value)
        {
            if (value == null)
            {
                Write(Byte_Zero);
                return;
            }

            int byteLength = value.Length << 1;
            CheckAndReSize(byteLength + 5);

            Write(Byte_One);
            Write(byteLength);

            Buffer.BlockCopy(value, 0, Bytes, PointAt, byteLength);

            PointAt += byteLength;
        }




        public static void WriteAt(byte[] bytes, int index, Char[] value)
        {
            if (value == null)
            {
                bytes[index] = Byte_Zero;
                return;
            }

            int byteLength = value.Length << 1;

            bytes[index] = Byte_One;
            WriteAt(bytes, index + 1, byteLength);

            Buffer.BlockCopy(value, 0, bytes, index + 5, byteLength);

        }
        public void Write(Char[] value)
        {
            if (value == null)
            {
                Write(Byte_Zero);
                return;
            }

            int byteLength = value.Length << 1;
            CheckAndReSize(byteLength + 5);

            Write(Byte_One);
            Write(byteLength);

            Buffer.BlockCopy(value, 0, Bytes, PointAt, byteLength);

            PointAt += byteLength;
        }






        public static void WriteAt(byte[] bytes, int index, UInt32[] value)
        {
            if (value == null)
            {
                bytes[index] = Byte_Zero;
                return;
            }

            int byteLength = value.Length << 2;

            bytes[index] = Byte_One;
            WriteAt(bytes, index + 1, byteLength);

            Buffer.BlockCopy(value, 0, bytes, index + 5, byteLength);

        }
        public void Write(UInt32[] value)
        {
            if (value == null)
            {
                Write(Byte_Zero);
                return;
            }

            int byteLength = value.Length << 2;
            CheckAndReSize(byteLength + 5);

            Write(Byte_One);
            Write(byteLength);

            Buffer.BlockCopy(value, 0, Bytes, PointAt, byteLength);

            PointAt += byteLength;
        }





        public static void WriteAt(byte[] bytes, int index, Int32[] value)
        {
            if (value == null)
            {
                bytes[index] = Byte_Zero;
                return;
            }

            int byteLength = value.Length << 2;

            bytes[index] = Byte_One;
            WriteAt(bytes, index + 1, byteLength);

            Buffer.BlockCopy(value, 0, bytes, index + 5, byteLength);
        }
        public void Write(Int32[] value)
        {
            if (value == null)
            {
                Write(Byte_Zero);
                return;
            }

            int byteLength = value.Length << 2;
            CheckAndReSize(byteLength + 5);

            Write(Byte_One);
            Write(byteLength);

            Buffer.BlockCopy(value, 0, Bytes, PointAt, byteLength);

            PointAt += byteLength;
        }




        public static void WriteAt(byte[] bytes, int index, UInt64[] value)
        {
            if (value == null)
            {
                bytes[index] = Byte_Zero;
                return;
            }

            int byteLength = value.Length << 3;

            bytes[index] = Byte_One;
            WriteAt(bytes, index + 1, byteLength);

            Buffer.BlockCopy(value, 0, bytes, index + 5, byteLength);
        }
        public void Write(UInt64[] value)
        {
            if (value == null)
            {
                Write(Byte_Zero);
                return;
            }

            int byteLength = value.Length << 3;
            CheckAndReSize(byteLength + 5);

            Write(Byte_One);
            Write(byteLength);

            Buffer.BlockCopy(value, 0, Bytes, PointAt, byteLength);

            PointAt += byteLength;
        }



        public static void WriteAt(byte[] bytes, int index, Int64[] value)
        {
            if (value == null)
            {
                bytes[index] = Byte_Zero;
                return;
            }

            int byteLength = value.Length << 3;

            bytes[index] = Byte_One;
            WriteAt(bytes, index + 1, byteLength);

            Buffer.BlockCopy(value, 0, bytes, index + 5, byteLength);
        }
        public void Write(Int64[] value)
        {
            if (value == null)
            {
                Write(Byte_Zero);
                return;
            }

            int byteLength = value.Length << 3;
            CheckAndReSize(byteLength + 5);

            Write(Byte_One);
            Write(byteLength);

            Buffer.BlockCopy(value, 0, Bytes, PointAt, byteLength);

            PointAt += byteLength;
        }




        public static void WriteAt(byte[] bytes, int index, Single[] value)
        {
            if (value == null)
            {
                bytes[index] = Byte_Zero;
                return;
            }

            int byteLength = value.Length << 2;

            bytes[index] = Byte_One;
            WriteAt(bytes, index + 1, byteLength);

            Buffer.BlockCopy(value, 0, bytes, index + 5, byteLength);
        }
        public void Write(Single[] value)
        {
            if (value == null)
            {
                Write(Byte_Zero);
                return;
            }

            int byteLength = value.Length << 2;
            CheckAndReSize(byteLength + 5);

            Write(Byte_One);
            Write(byteLength);

            Buffer.BlockCopy(value, 0, Bytes, PointAt, byteLength);

            PointAt += byteLength;
        }




        public static void WriteAt(byte[] bytes, int index, Double[] value)
        {
            if (value == null)
            {
                bytes[index] = Byte_Zero;
                return;
            }

            int byteLength = value.Length << 3;

            bytes[index] = Byte_One;
            WriteAt(bytes, index + 1, byteLength);

            Buffer.BlockCopy(value, 0, bytes, index + 5, byteLength);
        }
        public void Write(Double[] value)
        {
            if (value == null)
            {
                Write(Byte_Zero);
                return;
            }

            int byteLength = value.Length << 3;
            CheckAndReSize(byteLength + 5);

            Write(Byte_One);
            Write(byteLength);

            Buffer.BlockCopy(value, 0, Bytes, PointAt, byteLength);

            PointAt += byteLength;
        }






        public static void WriteAt(byte[] bytes, int index, Decimal[] value)
        {
            if (value == null)
            {
                bytes[index] = Byte_Zero;
                return;
            }

            int o_Length = value.Length;

            int byteLength = o_Length << 4;

            bytes[index] = Byte_One;
            WriteAt(bytes, index + 1, byteLength);

            for (int i = 0; i < o_Length; i++)
            {
                WriteAt(bytes, i * 16 + index + 5, value[i]);
            }
        }
        public void Write(Decimal[] value)
        {
            if (value == null)
            {
                Write(Byte_Zero);
                return;
            }

            int o_Length = value.Length;

            int byteLength = o_Length << 4;
            CheckAndReSize(byteLength + 5);

            Write(Byte_One);
            Write(byteLength);

            for (int i = 0; i < o_Length; i++)
            {
                Write(value[i]);
            }
        }


        public static void WriteAt(byte[] bytes, int index, Guid[] value)
        {
            if (value == null)
            {
                bytes[index] = Byte_Zero;
                return;
            }

            int o_Length = value.Length;

            int byteLength = o_Length << 4;

            bytes[index] = Byte_One;
            WriteAt(bytes, index + 1, byteLength);

            for (int i = 0; i < o_Length; i++)
            {
                WriteAt(bytes, i * 16 + index + 5, value[i]);
            }
        }
        public void Write(Guid[] value)
        {
            if (value == null)
            {
                Write(Byte_Zero);
                return;
            }

            int o_Length = value.Length;

            int byteLength = o_Length << 4;
            CheckAndReSize(byteLength + 5);

            Write(Byte_One);
            Write(byteLength);

            for (int i = 0; i < o_Length; i++)
            {
                Write(value[i]);
            }
        }




        public static void WriteAt(byte[] bytes, int index, DateTime[] value)
        {
            if (value == null)
            {
                bytes[index] = Byte_Zero;
                return;
            }

            int o_Length = value.Length;

            int byteLength = o_Length << 3;

            bytes[index] = Byte_One;
            WriteAt(bytes, index + 1, byteLength);

            for (int i = 0; i < o_Length; i++)
            {
                WriteAt(bytes, i * 8 + index + 5, value[i]);
            }
        }
        public void Write(DateTime[] value)
        {
            if (value == null)
            {
                Write(Byte_Zero);
                return;
            }

            int o_Length = value.Length;
            
            int byteLength = o_Length << 3;
            CheckAndReSize(byteLength + 5);

            Write(Byte_One);
            Write(byteLength);

            for (int i = 0; i < o_Length; i++)
            {
                Write(value[i]);
            }
        }





        public static void WriteAt(byte[] bytes, int index, TimeSpan[] value)
        {
            if (value == null)
            {
                bytes[index] = Byte_Zero;
                return;
            }

            int o_Length = value.Length;
            int byteLength = o_Length << 3;

            bytes[index] = Byte_One;
            WriteAt(bytes, index + 1, byteLength);

            for (int i = 0; i < o_Length; i++)
            {
                WriteAt(bytes, i * 8 + index + 5, value[i]);
            }
        }
        public void Write(TimeSpan[] value)
        {
            if (value == null)
            {
                Write(Byte_Zero);
                return;
            }

            int o_Length = value.Length;
                        
            int byteLength = o_Length << 3;
            CheckAndReSize(byteLength + 5);

            Write(Byte_One);
            Write(byteLength);

            for (int i = 0; i < o_Length; i++)
            {
                Write(value[i]);
            }
        }




        /// <summary>
        /// 将string值以原始char[]形式写入，不做任何编码检查和转换
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public static void WriteAt(byte[] bytes, int index, string value)
        {
            if (value == null)
            {
                bytes[index] = Byte_Zero;
                return;
            }

            int byteLength = value.Length << 1;

            bytes[index] = Byte_One;
            WriteAt(bytes, index + 1, byteLength);

            Buffer.BlockCopy(value.ToCharArray(0, value.Length), 0, bytes, index + 5, byteLength);
        }
        /// <summary>
        /// 将string值以原始char[]形式写入，不做任何编码检查和转换
        /// </summary>
        /// <param name="value">不能为空</param>
        public void Write(string value)
        {
            if (value == null)
            {
                Write(Byte_Zero);
                return;
            }

            int byteLength = value.Length << 1;
            CheckAndReSize(byteLength + 5);

            Write(Byte_One);
            Write(byteLength);

            Buffer.BlockCopy(value.ToCharArray(0, value.Length), 0, Bytes, PointAt, byteLength);

            PointAt += byteLength;
        }
    }
}
