using System;

namespace HHW.Service
{
    public class MessagePakcer : IMessagePacker
    {
        public object DeserializeFrom(Type type, byte[] bytes)
        {
            return Serialization.DeserializeFrom(type, bytes, 0, out int readLength);
        }

        public object DeserializeFrom(Type type, byte[] bytes, int index, int count)
        {
            return Serialization.DeserializeFrom(type, bytes, index, count, out int readLength);
        }

        public T DeserializeFrom<T>(byte[] bytes)
        {
            return Serialization.DeserializeFrom<T>(bytes, 0, out int readLength);
        }

        public T DeserializeFrom<T>(byte[] bytes, int index, int count)
        {
            return Serialization.DeserializeFrom<T>(bytes, index, count, out int readLength);
        }

        public T DeserializeFrom<T>(string str)
        {
            throw new NotImplementedException();
        }

        public object DeserializeFrom(Type type, string str)
        {
            throw new NotImplementedException();
        }

        public byte[] Serialize(object obj)
        {
            byte[] buf = new byte[0];
            int len = Serialization.SerializeTo(obj, ref buf, 0);
            Array.Resize(ref buf, len);
            return buf;
        }

        public string SerializeToText(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
