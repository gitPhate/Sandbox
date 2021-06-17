using Confluent.Kafka;
using System;

namespace KafkaStudy
{
    static class PersonProducer
    {
        public static void Produce(string topic, ClientConfig config, Person person)
        {
            using (var producer = new ProducerBuilder<string, Person>(config).SetValueSerializer(new PersonSerializer()).Build())
            {
                string key = person.Id.ToString();

                Console.WriteLine($"Producing record: {key} {person}");

                producer.Produce(topic, new Message<string, Person> { Key = key, Value = person },
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
