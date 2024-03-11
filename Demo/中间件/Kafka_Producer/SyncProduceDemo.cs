using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kafka_Producer
{
    public class SyncProduceDemo
    {
        public static void Run(string server, string topic)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = server
            };


            using var producer = new ProducerBuilder<Null, string>(config).Build();

            var stopSatuch = Stopwatch.StartNew();
            try
            {
                //虽然此处使用ProduceAsync，其实是异步发送消息
                //因为每次发送消息后，无需等待。之后消息发送成功后会自动调用回调函数处理其他逻辑

                for (var i = 0; i < 100; i++)
                {
                    producer.Produce(topic, new Message<Null, string>
                    {
                        Value = $"Hello World! Kafka!-{i}",
                    },
                    report =>
                    {
                        Console.WriteLine($"已发送：[{report.Value}],Offset:{report.TopicPartitionOffset}");
                    });
                }
            }
            catch (ProduceException<string, string> e)
            {
                Console.WriteLine($"发送失败: {e.Error.Reason}");
            }
            //如果对性能要求不高的话，可以调用调用 producer.flush() 方法，该方法会将数据全部发送到Kafka，否则就会阻塞
            producer.Flush(CancellationToken.None);
            Console.WriteLine($"Produce 耗时：{stopSatuch.Elapsed}");
        }
    }
}
