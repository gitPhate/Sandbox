using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Confluent.Kafka;

namespace KafkaStudy
{
    class PersonDeserializer : IDeserializer<Person>
    {
        public Person Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (isNull)
            {
                return null;
            }

            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                ms.Write(data.ToArray(), 0, data.Length);
                ms.Seek(0, SeekOrigin.Begin);
                return bf.Deserialize(ms) as Person;
            }
        }
    }
}
