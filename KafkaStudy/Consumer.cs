using Confluent.Kafka;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;

namespace KafkaStudy
{
    static class Consumer
    {
        public static void Consume(string topic, ClientConfig config, CancellationToken cancellationToken)
        {
            var consumerConfig = new ConsumerConfig(config)
            {
                GroupId = "dotnet-example-group-1",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using (var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build())
            {
                consumer.Subscribe(topic);
                int totalCount = 0;
                try
                {
                    while (true)
                    {
                        ConsumeResult<string, string> cr = consumer.Consume(cancellationToken);
                        totalCount += JObject.Parse(cr.Message.Value).Value<int>("count");
                        Console.WriteLine($"Consumed record with key {cr.Message.Key} and value {cr.Message.Value}, and updated total count to {totalCount}");
                        //consumer.Commit(cr);
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ctrl-C was pressed.
                }
                finally
                {
                    consumer.Close();
                }
            }
        }
    }
}
