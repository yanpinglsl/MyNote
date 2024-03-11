using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kafka_Consumer
{
    internal class ResetOffset
    {
        public static void Run()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "192.168.2.13:9092",
                GroupId = "consumer.group.hello",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

            //位移维度：Assign
            // var topicPartition = new TopicPartition("sample", new Partition(0)); 
            // var tpOffset = new TopicPartitionOffset(topicPartition, new Offset(0));
            // consumer.Assign(tpOffset);

            //时间维度：DateTime
            var timestamp = new Timestamp(DateTime.Now.AddMinutes(-30));
            var dateTimeOffset = new TopicPartitionTimestamp(new TopicPartition("sample", new Partition(0)), timestamp);
            //var tpos = consumer.OffsetsForTimes(new[] { dateTimeOffset }, TimeSpan.FromSeconds(0));
            //consumer.Assign(tpos);

            //时间维度：Duration
            // var timestamp = new Timestamp(new DateTime(2023, 11, 1));
            // var dateTimeOffset = new TopicPartitionTimestamp(new TopicPartition("sample", new Partition(0)), timestamp);
            var tpos = consumer.OffsetsForTimes(new[] { dateTimeOffset }, TimeSpan.FromSeconds(5));
            consumer.Assign(tpos);

            //subscribe() 和 assign() 两个方法的使用是互斥的，只能使用其中之一
            //consumer.Subscribe("sample");

            Console.WriteLine("等待消息……");
            while (true)
            {
                var consumeResult = consumer.Consume();

                //位移维度：Seek
                // var offsets = consumer.Committed(TimeSpan.FromSeconds(5));                //
                // consumer.Seek(new TopicPartitionOffset(consumeResult.TopicPartition, new Offset(offsets[0].Offset - 2)));                //
                // consumer.Seek(new TopicPartitionOffset(consumeResult.TopicPartition, new Offset(offsets[0].Offset)));                //
                // consumer.Seek(new TopicPartitionOffset(consumeResult.TopicPartition,Offset.Beginning));

                Console.WriteLine($"收到消息：[{consumeResult.Message.Value}] 来自 [{consumeResult.TopicPartitionOffset}]");
            }
        }
    }
}
