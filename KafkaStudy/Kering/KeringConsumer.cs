using Confluent.Kafka;
using System;
using System.Threading;

namespace KafkaStudy.Kering
{
    static class KeringConsumer
    {
        public static void Consume(string topic, string groupId, ClientConfig config, CancellationToken cancellationToken)
        {
            var consumerConfig = new ConsumerConfig(config)
            {
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using (var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build())
            {
                consumer.Subscribe(topic);
                Console.WriteLine($"Subscribed to topic {topic}");

                try
                {
                    while (true)
                    {
                        ConsumeResult<string, string> consumeResult = consumer.Consume(cancellationToken);
                        Console.WriteLine($"Consumed record with key {consumeResult.Message.Key} and value {consumeResult.Message.Value}");
                        if (CanCommitMessage(consumeResult.Message))
                        {
                            consumer.Commit(consumeResult);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ctrl-C was pressed.
                }
                finally
                {
                    consumer.Unsubscribe();
                    consumer.Close();
                }
            }
        }

        private static bool CanCommitMessage(Message<string, string> message)
        {
            return false;
        }
    }
}
