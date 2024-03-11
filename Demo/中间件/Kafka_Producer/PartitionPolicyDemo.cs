using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kafka_Producer
{
    public static class PartitionPolicyDemo
    {
        /// <summary>
        /// 轮询策略
        /// </summary>
        /// <param name="server"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        public static async Task RoundRobin(string server, string topic)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = server
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();

            // 获取指定主题的分区数量
            var partCount = KafkaHelper.Instance.GetPartitionCount(server, topic);

            for (var i = 0; i < 10; i++)
            {
                // 使用取余操作来实现轮询分区策略
                var partNum = new Partition(i % partCount);

                var message = new Message<Null, string> { Value = $"Hello, Kafka!{i}" };
                var result = await producer.ProduceAsync(new TopicPartition(topic, new Partition(partNum)), message);

                Console.WriteLine($"消息已发送[{result.Value}]到[{result.TopicPartitionOffset}]");
            }
        }

        /// <summary>
        /// 随机策略
        /// </summary>
        /// <param name="server"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        public static async Task Random(string server, string topic)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = server
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();

            // 获取指定主题的分区数量
            var partCount = KafkaHelper.Instance.GetPartitionCount(server, topic);
            // 创建随机对象
            var random = new Random();

            for (var i = 0; i < 10; i++)
            {
                // 获取随机分区
                var partNum = random.Next(0, partCount);

                var message = new Message<Null, string> { Value = $"Hello, Kafka!{i}" };
                var result = await producer.ProduceAsync(new TopicPartition(topic, new Partition(partNum)), message);

                Console.WriteLine($"消息已发送[{result.Value}]到[{result.TopicPartitionOffset}]");
            }
        }
    }
}
