using System;
using System.IO;
#if Server
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
#endif

namespace GN
{
    public class BsonPacker : IMessagePacker
    {
#if Server
        public object DeserializeFrom(Type type, byte[] bytes)
        {
            return BsonSerializer.Deserialize(bytes, type);
        }

        public object DeserializeFrom(Type type, byte[] bytes, int index, int count)
        {
            using (MemoryStream memoryStream = new MemoryStream(bytes, index, count))
            {
                return BsonSerializer.Deserialize(memoryStream, type);
            }
        }

        public T DeserializeFrom<T>(byte[] bytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                return (T)BsonSerializer.Deserialize(memoryStream, typeof(T));
            }
        }

        public T DeserializeFrom<T>(byte[] bytes, int index, int count)
        {
            return (T)DeserializeFrom(typeof(T), bytes, index, count);
        }

        public T DeserializeFrom<T>(string str)
        {
            return BsonSerializer.Deserialize<T>(str);
        }

        public object DeserializeFrom(Type type, string str)
        {
            return BsonSerializer.Deserialize(str, type);
        }

        public byte[] Serialize(object obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var bsonWriter = new BsonBinaryWriter(memoryStream))
                {
                    BsonSerializer.Serialize(bsonWriter, obj.GetType(), obj);
                }
                return memoryStream.ToArray();
            }
        }

        public string SerializeToText(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var bsonWriter = new JsonWriter(stringWriter))
                {
                    BsonSerializer.Serialize(bsonWriter, obj.GetType(), obj);
                }
                return stringWriter.ToString();
            }
        }
#else
        public object DeserializeFrom(Type type, byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public object DeserializeFrom(Type type, byte[] bytes, int index, int count)
        {
            throw new NotImplementedException();
        }

        public T DeserializeFrom<T>(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public T DeserializeFrom<T>(byte[] bytes, int index, int count)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public string SerializeToText(object obj)
        {
            throw new NotImplementedException();
        }
#endif
    }
}
