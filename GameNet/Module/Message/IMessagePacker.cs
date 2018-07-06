using System;

namespace GN
{
    public interface IMessagePacker
    {
        byte[] Serialize(object obj);
        string SerializeToText(object obj);

        object DeserializeFrom(Type type, byte[] bytes);
        object DeserializeFrom(Type type, byte[] bytes, int index, int count);
        T DeserializeFrom<T>(byte[] bytes);
        T DeserializeFrom<T>(byte[] bytes, int index, int count);

        T DeserializeFrom<T>(string str);
        object DeserializeFrom(Type type, string str);
    }
}
