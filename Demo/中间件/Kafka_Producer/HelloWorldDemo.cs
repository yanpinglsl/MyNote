using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Confluent.Kafka.ConfigPropertyNames;

namespace Kafka_Producer
{
    public static class HelloWorldDemo
    {
        public static async void Run(string server,string topic)
        {
            var conif = new ProducerConfig()
            {
                BootstrapServers = server,
                //None:生产者发送消息之后不需要等待任何服务端的响应
                //Leader:产者发送消息之后，只要分区的 leader 副本成功写入消息，那么它就会收到来自服务端的成功响应
                //All:生产者在消息发送之后，需要等待 ISR 中的所有副本都成功写入消息之后才能够收到来自服务端的成功响应
                Acks = Acks.All 
            };
            var message = "Hello World! Kafka!";
            var producer = new ProducerBuilder<string, string>(conif).Build();
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    //如果topic不存在，则Kafka会自动创建，默认分区数为1
                    var result = await producer.ProduceAsync(topic, new Message<string, string>
                    {
                        Key = "hello",
                        Value = message
                    });
                    Console.WriteLine($"消息已发送[{result.Value}]到[{result.TopicPartitionOffset}]");
                }

            }
            catch (ProduceException<string,string> e)
            {
                Console.WriteLine($"发送失败: {e.Error.Reason}");
            }catch(Exception e)
            {

                Console.WriteLine($"发送失败: {e.InnerException}");
            }
        }
    }
}
