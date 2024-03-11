using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kafka_Producer
{
    public class AsyncProduceDemo
    {
        public static async Task Run(string server, string topic)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = server,
                CompressionType = CompressionType.None,
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();

            var stopSatuch = Stopwatch.StartNew();
            try
            {
                //虽然此处使用ProduceAsync，其实是同步发送消息
                //因为每次都需要等待消息成功，才发送下一条
                for (var i = 0; i < 100; i++)
                {
                    var messageValue = $"Hello World! Kafka!-{i}";

                    var result = await producer.ProduceAsync(topic, new Message<Null, string>
                    {
                        Value = messageValue
                    });
                    Console.WriteLine($"消息已发送[{result.Value}]到[{result.TopicPartitionOffset}]");
                }

            }
            catch (ProduceException<string, string> e)
            {
                Console.WriteLine($"发送失败: {e.Error.Reason}");
            }

            Console.WriteLine($"ProduceAsync 耗时：{stopSatuch.Elapsed}");
        }
    }
}
