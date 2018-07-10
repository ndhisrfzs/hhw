using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.IO;

namespace GN
{
    public class BsonPacker : IMessagePacker
    {
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
    }
}
