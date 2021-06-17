using Confluent.Kafka;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace KafkaStudy
{
    class PersonSerializer : ISerializer<Person>
    {
        public byte[] Serialize(Person data, SerializationContext context)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, data);
                return ms.ToArray();
            }
        }
    }
}
