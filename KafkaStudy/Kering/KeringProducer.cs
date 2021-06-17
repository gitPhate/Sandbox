using Confluent.Kafka;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace KafkaStudy.Kering
{
    static class KeringProducer
    {
        public static void Produce(string topic, ClientConfig config)
        {
            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                string key = "person";
                var person = new {
                    FirstName = "Franz",
                    LastName = "Kafka",
                    Age = 22
                };
                string value = JObject.FromObject(person).ToString(Formatting.None);

                Console.WriteLine($"Producing record: {key} {value}");

                producer.Produce(topic, new Message<string, string> { Key = key, Value = value },
                    (deliveryReport) =>
                    {
                        if (deliveryReport.Error.Code != ErrorCode.NoError)
                        {
                            Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                        }
                        else
                        {
                            Console.WriteLine($"Produced message to: {deliveryReport.TopicPartitionOffset}");
                        }
                    });

                producer.Flush(TimeSpan.FromSeconds(10));

                Console.WriteLine($"Message was produced to topic {topic}");
            }
        }
    }
}
