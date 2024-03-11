using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kafka_Consumer
{
    public static class HelloWorldDemo
    {
        public static void Run(string server,string topic)
        {
            string groupID = "consumer.group.hello";
            var conif = new ConsumerConfig()
            {
                BootstrapServers = server,
                GroupId = groupID,
                AutoOffsetReset = AutoOffsetReset.Earliest 
            };
            var consumer = new ConsumerBuilder<Ignore, string>(conif).Build();
            consumer.Subscribe(topic);
            Console.WriteLine("等待消息");
            while (true)
            {
                var consumeResult = consumer.Consume();
                Console.WriteLine($"收到消息：[{consumeResult.Message.Value}] 来自 [{consumeResult.TopicPartitionOffset}]");
            }
        }
    }
}
