using System;
using System.Collections.Generic;
using System.Text;

namespace HHW.Service
{
    public interface IMessagePacker
    {
        byte[] Serialize(object obj);
        object DeserializeFrom(Type type, byte[] bytes, int index, int count);
        T DeserializeFrom<T>(byte[] bytes);
    }
}
