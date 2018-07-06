using System;
using System.Runtime.InteropServices;

namespace GN
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct V32
    {
        [FieldOffset(0)]
        public UInt32 U32;

        [NonSerialized]
        [FieldOffset(0)]
        public Int32 I32;

        [NonSerialized]
        [FieldOffset(0)]
        public Single S;

        [NonSerialized]
        [FieldOffset(0)]
        public byte B0;

        [NonSerialized]
        [FieldOffset(1)]
        public byte B1;

        [NonSerialized]
        [FieldOffset(2)]
        public byte B2;

        [NonSerialized]
        [FieldOffset(3)]
        public byte B3;

        /// <summary>
        /// 将小位置前
        /// </summary>
        public UInt32 Reverse
        {
            get
            {
                return (uint)B3 | (uint)B2 << 8 | (uint)B1 << 16 | (uint)B0 << 24;
            }
        }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct V64
    {
        [FieldOffset(0)]
        public UInt64 U64;

        [NonSerialized]
        [FieldOffset(0)]
        public Int64 I64;

        [NonSerialized]
        [FieldOffset(0)]
        public Double D;

        [NonSerialized]
        [FieldOffset(0)]
        public DateTime T;

        [NonSerialized]
        [FieldOffset(0)]
        public TimeSpan TS;

        [NonSerialized]
        [FieldOffset(0)]
        public Single S1;

        [NonSerialized]
        [FieldOffset(4)]
        public Single S2;

        [NonSerialized]
        [FieldOffset(0)]
        public byte B0;

        [NonSerialized]
        [FieldOffset(1)]
        public byte B1;

        [NonSerialized]
        [FieldOffset(2)]
        public byte B2;

        [NonSerialized]
        [FieldOffset(3)]
        public byte B3;

        [NonSerialized]
        [FieldOffset(4)]
        public byte B4;

        [NonSerialized]
        [FieldOffset(5)]
        public byte B5;

        [NonSerialized]
        [FieldOffset(6)]
        public byte B6;

        [NonSerialized]
        [FieldOffset(7)]
        public byte B7;


        /// <summary>
        /// 将小位置前
        /// </summary>
        public UInt64 Reverse
        {
            get
            {
                return (uint)B7 | (uint)B6 << 8 | (uint)B5 << 16 | (uint)B4 << 24 | (ulong)B3 << 32 | (ulong)B2 << 40 | (ulong)B1 << 48 | (ulong)B0 << 56;
            }
        }
    }



    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct V128
    {
        [FieldOffset(0)]
        public UInt64 U64_0;

        [FieldOffset(8)]
        public UInt64 U64_1;

        [NonSerialized]
        [FieldOffset(0)]
        public Int64 I64_0;

        [NonSerialized]
        [FieldOffset(8)]
        public Int64 I64_1;

        [NonSerialized]
        [FieldOffset(0)]
        public Double D_0;

        [NonSerialized]
        [FieldOffset(8)]
        public Double D_1;

        [NonSerialized]
        [FieldOffset(0)]
        public DateTime T_0;

        [NonSerialized]
        [FieldOffset(8)]
        public DateTime T_1;

        [NonSerialized]
        [FieldOffset(0)]
        public TimeSpan TS_0;

        [NonSerialized]
        [FieldOffset(8)]
        public TimeSpan TS_1;

        [NonSerialized]
        [FieldOffset(0)]
        public UInt32 U32_0;

        [NonSerialized]
        [FieldOffset(4)]
        public UInt32 U32_1;

        [NonSerialized]
        [FieldOffset(8)]
        public UInt32 U32_2;

        [NonSerialized]
        [FieldOffset(12)]
        public UInt32 U32_3;

        [NonSerialized]
        [FieldOffset(0)]
        public Int32 I32_0;

        [NonSerialized]
        [FieldOffset(4)]
        public Int32 I32_1;

        [NonSerialized]
        [FieldOffset(8)]
        public Int32 I32_2;

        [NonSerialized]
        [FieldOffset(12)]
        public Int32 I32_3;

        [NonSerialized]
        [FieldOffset(0)]
        public Single S1;

        [NonSerialized]
        [FieldOffset(4)]
        public Single S2;

        [NonSerialized]
        [FieldOffset(8)]
        public Single S3;

        [NonSerialized]
        [FieldOffset(12)]
        public Single S4;

        [NonSerialized]
        [FieldOffset(0)]
        public Decimal D;

        [NonSerialized]
        [FieldOffset(0)]
        public Guid G;

        [NonSerialized]
        [FieldOffset(0)]
        public byte B00;

        [NonSerialized]
        [FieldOffset(1)]
        public byte B01;

        [NonSerialized]
        [FieldOffset(2)]
        public byte B02;

        [NonSerialized]
        [FieldOffset(3)]
        public byte B03;

        [NonSerialized]
        [FieldOffset(4)]
        public byte B04;

        [NonSerialized]
        [FieldOffset(5)]
        public byte B05;

        [NonSerialized]
        [FieldOffset(6)]
        public byte B06;

        [NonSerialized]
        [FieldOffset(7)]
        public byte B07;

        [NonSerialized]
        [FieldOffset(8)]
        public byte B08;

        [NonSerialized]
        [FieldOffset(9)]
        public byte B09;

        [NonSerialized]
        [FieldOffset(10)]
        public byte B10;

        [NonSerialized]
        [FieldOffset(11)]
        public byte B11;

        [NonSerialized]
        [FieldOffset(12)]
        public byte B12;

        [NonSerialized]
        [FieldOffset(13)]
        public byte B13;

        [NonSerialized]
        [FieldOffset(14)]
        public byte B14;

        [NonSerialized]
        [FieldOffset(15)]
        public byte B15;


        ///// <summary>
        ///// 将小位置前
        ///// </summary>
        //public UInt64 Reverse
        //{
        //    get
        //    {
        //        return (uint)B7 | (uint)VB6 << 8 | (uint)VB5 << 16 | (uint)VB4 << 24 | (ulong)VB3 << 32 | (ulong)VB2 << 40 | (ulong)VB1 << 48 | (ulong)VB0 << 56;
        //    }
        //}
    }
}
