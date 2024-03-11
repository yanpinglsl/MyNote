using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kafka_Consumer
{
    public class ManualCommitDemo
    {
        public static void Run(string server, string topic)
        {
            string groupID = "consumer.group.hello";
            var conif = new ConsumerConfig()
            {
                BootstrapServers = server,
                GroupId = groupID,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,//是否启动自动提交
                //AutoCommitIntervalMs = 1000,//自动提交的间隔时间
                //其他参数
                //MaxPartitionFetchBytes = 100,
                //IsolationLevel = IsolationLevel.ReadCommitted ,
                //MaxPollIntervalMs = 100,
                // 拉取的最小数据量
                //FetchMinBytes = 10,
                //FetchMaxBytes = 1,
                //FetchWaitMaxMs = 500,
            };
            var consumer = new ConsumerBuilder<Ignore, string>(conif).Build();
            consumer.Subscribe(topic);
            Console.WriteLine("等待消息");
            while (true)
            {
                var consumeResult = consumer.Consume();
                Console.WriteLine($"收到消息：[{consumeResult.Message.Value}] 来自 [{consumeResult.TopicPartitionOffset}]");
                // 处理完成后手动提交位移
                //此处
                consumer.Commit(consumeResult);
            }
        }
    }
}
