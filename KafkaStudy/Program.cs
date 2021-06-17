using Confluent.Kafka;
using KafkaStudy.Kering;
using System.Threading;

namespace KafkaStudy
{
    class Program
    {
        // https://github.com/confluentinc/examples/blob/latest/clients/cloud/csharp/Program.cs

        static void Main(string[] args)
        {
            ClientConfig clientConfig =
                new ClientConfig
                {
                    BootstrapServers = "kafka1.studio.dev4apps.io:9092,kafka2.studio.dev4apps.io:9092,kafka3.studio.dev4apps.io:9092,kafka4.studio.dev4apps.io:9092,kafka5.studio.dev4apps.io:9092,kafka6.studio.dev4apps.io:9092",
                    SecurityProtocol = SecurityProtocol.Plaintext,
                    SaslMechanism = null,
                    ClientId = "Globe_dotNet_returns"
                };

            string topic = "TOTO";
            string groupId = "CF_Globe_dotNet_returns";

            //KeringProducer.Produce(topic, clientConfig);
            KeringConsumer.Consume(topic, groupId, clientConfig, default(CancellationToken));
        }
    }
}
