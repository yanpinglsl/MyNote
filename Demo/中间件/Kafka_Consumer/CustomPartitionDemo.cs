using Confluent.Kafka;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kafka_Consumer
{
   /// <summary>
   /// 消费指定分区的数据
   /// </summary>
    public class CustomPartitionDemo
    {
        public static void Run(string server, string topic)
        {
            string groupID = "consumer.group.tt";
            var conif = new ConsumerConfig()
            {
                BootstrapServers = server,
                GroupId = groupID,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            var consumer = new ConsumerBuilder<Ignore, string>(conif).Build();
            //consumer.Subscribe(topic);
            // 指定要消费的分区
            var partition = new TopicPartition(topic, 1);
            consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(partition, Offset.Beginning) });
            Console.WriteLine("等待消息");
            while (true)
            {
                var consumeResult = consumer.Consume();
                Console.WriteLine($"收到消息：[{consumeResult.Message.Value}] 来自 [{consumeResult.TopicPartitionOffset}]");
            }
        }
    }
}
