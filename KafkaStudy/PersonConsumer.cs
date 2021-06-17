using Confluent.Kafka;
using System;
using System.Threading;

namespace KafkaStudy
{
    static class PersonConsumer
    {
        public static void Consume(string topic, ClientConfig config)
        {
            var consumerConfig = new ConsumerConfig(config)
            {
                GroupId = "dotnet-example-group-1",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };

            using (var consumer = new ConsumerBuilder<string, Person>(consumerConfig).SetValueDeserializer(new PersonDeserializer()).Build())
            {
                consumer.Subscribe(topic);
                try
                {
                    while (true)
                    {
                        ConsumeResult<string, Person> cr = consumer.Consume(cts.Token);
                        Console.WriteLine($"Consumed record with key {cr.Message.Key} and value {cr.Message.Value}");
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
