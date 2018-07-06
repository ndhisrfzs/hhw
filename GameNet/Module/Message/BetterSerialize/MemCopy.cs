#if Point
using System;

namespace GN
{
    public static class MemoryCopy
    {
        /// <summary>
        /// MemCopy byte[] to byte[], use Int64 block
        /// </summary>
        /// <param name="src"></param>
        /// <param name="srcStart"></param>
        /// <param name="dest"></param>
        /// <param name="destStart"></param>
        /// <param name="len"></param>
        public static unsafe void MemCopy(Byte[] src, Int32 srcStart, Byte[] dest, Int32 destStart, Int32 len)
        {
            if (len < 0)
            {
                throw new Exception("len must >= 0");
            }
            if (len == 0)
            {
                return;
            }

            Int32 i1 = src.Length - srcStart;
            if (len > i1)
            {
                throw new Exception("wrong srcStart or len");
            }

            Int32 i2 = dest.Length - destStart;
            if (len > i2)
            {
                throw new Exception("wrong destStart or len");
            }

            fixed (Byte* p_src = src)
            {
                fixed (Byte* p_dest = dest)
                {
                    MemCopy(p_src + srcStart, p_dest + destStart, len);
                }
            }
        }



#if Server
        ///// <summary>
        ///// MemCopy byte[] to byte[], use Int64 block
        ///// </summary>
        ///// <param name="src"></param>
        ///// <param name="srcStart"></param>
        ///// <param name="dest"></param>
        ///// <param name="destStart"></param>
        ///// <param name="len"></param>
        public static unsafe void MemCopy(Byte[] src, Int64 srcStart, Byte[] dest, Int64 destStart, Int64 len)
        {
            if (len < 0)
            {
                throw new Exception("len must >= 0");
            }
            if (len == 0)
            {
                return;
            }

            Int64 i1 = src.LongLength - srcStart;
            if (len > i1)
            {
                throw new Exception("wrong srcStart or len");
            }

            Int64 i2 = dest.LongLength - destStart;
            if (len > i2)
            {
                throw new Exception("wrong destStart or len");
            }

            fixed (Byte* p_src = src)
            {
                fixed (Byte* p_dest = dest)
                {
                    MemCopy(p_src + srcStart, p_dest + destStart, len);
                }
            }
        }
#endif





        /// <summary>
        /// Copy memory with point, use Int64 block
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="len"></param>
        public static unsafe void MemCopy(Byte* src, Byte* dest, Int32 len)
        {

            if (len >= 16)  //DDR类内存总线为64位，16×4 = 64
            {
                do
                {
                    *((Int64*)dest) = *((Int64*)src);
                    *((Int64*)(dest + 8)) = *((Int64*)(src + 8));
                    dest += 16;
                    src += 16;
                }
                while ((len -= 16) >= 16);
            }
            if (len > 0)
            {
                if ((len & 8) != 0)
                {
                    *((Int64*)dest) = *((Int64*)src);
                    dest += 8;
                    src += 8;
                }
                if ((len & 4) != 0)
                {
                    *((Int32*)dest) = *((Int32*)src);
                    dest += 4;
                    src += 4;
                }
                if ((len & 2) != 0)
                {
                    *((Int16*)dest) = *((Int16*)src);
                    dest += 2;
                    src += 2;
                }
                if ((len & 1) != 0)
                {
                    //.NET源码这里错误，必须注释掉这两行
                    //dest++;
                    //src++;
                    dest[0] = src[0];
                }
            }
        }



        /// <summary>
        /// Copy memory with point, use Int64 block
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="len"></param>
        public static unsafe void MemCopy(Byte* src, Byte* dest, Int64 len)
        {

            if (len >= 16)  //DDR类内存总线为64位，16×4 = 64
            {
                do
                {
                    *((Int64*)dest) = *((Int64*)src);
                    *((Int64*)(dest + 8)) = *((Int64*)(src + 8));
                    dest += 16;
                    src += 16;
                }
                while ((len -= 16) >= 16);
            }
            if (len > 0)
            {
                if ((len & 8) != 0)
                {
                    *((Int64*)dest) = *((Int64*)src);
                    dest += 8;
                    src += 8;
                }
                if ((len & 4) != 0)
                {
                    *((Int32*)dest) = *((Int32*)src);
                    dest += 4;
                    src += 4;
                }
                if ((len & 2) != 0)
                {
                    *((Int16*)dest) = *((Int16*)src);
                    dest += 2;
                    src += 2;
                }
                if ((len & 1) != 0)
                {
                    //.NET源码这里错误，必须注释掉这两行
                    //dest++;
                    //src++;
                    dest[0] = src[0];
                }
            }
        }
    }
}
#endif
