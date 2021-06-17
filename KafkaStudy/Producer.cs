using Confluent.Kafka;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace KafkaStudy
{
    static class Producer
    {
        public static void Produce(string topic, ClientConfig config)
        {
            ProducerConfig producerConfig = new ProducerConfig(config)
            {
                
            };

            using (var producer = new ProducerBuilder<string, string>(producerConfig).Build())
            {
                int numProduced = 0;
                int numMessages = 1;
                for (int i = 0; i < numMessages; ++i)
                {
                    string key = "Dorel";
                    string val = JObject.FromObject(new { count = i, message = "This is just a test." }).ToString(Formatting.None);

                    Console.WriteLine($"Producing record: {key} {val}");

                    producer.Produce(topic, new Message<string, string> { Key = key, Value = val },
                        (deliveryReport) =>
                        {
                            if (deliveryReport.Error.Code != ErrorCode.NoError)
                            {
                                Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                            }
                            else
                            {
                                Console.WriteLine($"Produced message to: {deliveryReport.TopicPartitionOffset}");
                                numProduced += 1;
                            }
                        });
                }

                producer.Flush(TimeSpan.FromSeconds(10));

                Console.WriteLine($"{numProduced} messages were produced to topic {topic}");
            }
        }
    }
}
